#r "/Users/hsiang-yuanliao/.nuget/packages/akka/1.4.10/lib/netstandard2.0/Akka.dll"
#r "/Users/hsiang-yuanliao/.nuget/packages/akka.fsharp/1.4.10/lib/netstandard2.0/Akka.FSharp.dll"
#r "/Users/hsiang-yuanliao/.nuget/packages/newtonsoft.json/12.0.3/lib/net45/Newtonsoft.Json.dll"
#r "/Users/hsiang-yuanliao/.nuget/packages/fspickler/5.3.2/lib/net45/FsPickler.dll"

#time "on"
open System
open Akka
open Akka.FSharp


type ProcessorMessage = InputMsg of uint64 * uint64
type ProcessorMessage2 = InputMsg2 of uint64 * uint64 * uint64



let SumConsecSquare a b = 
    (( ( a + b - 1UL ) * ( a + b ) * ( 2UL * a + 2UL * b - 1UL ) ) / 6UL) - (( ( a - 1UL ) * a * ( 2UL * a - 1UL) ) / 6UL)
    
let isPerfectSquare x =
    let h = x &&& 0xFUL
    if (h > 9UL) then false
            else
              if ( h <> 2UL && h <> 3UL && h <> 5UL && h <> 6UL && h <> 7UL && h <> 8UL ) then
                let t = ((x |> double |> sqrt) + 0.5) |> floor|> int |>uint64
                t*t = x
              else false
             




let system = System.create "system" <| Configuration.defaultConfig()

let spawnChild childActor name =
    spawn system name childActor

let sendMsg tActor (InputMsg ( x , y))= 
    tActor <! InputMsg(x, y)

let sendMsg2 tActor (InputMsg2 (x, y, z))= 
    tActor <! InputMsg2(x, y, z)

let sendStrMsg tActor (str:string) =
    tActor <! str



let myActor (mailbox: Actor<_>) =
    let rec loop() = actor {
        let! InputMsg2(head, tail, k) = mailbox.Receive()
        //printfn "worker acotr receive msg"
     
        if (head = 0UL) && (tail = 0UL) && (k = 0UL) then
            //printfn "Initial done with k=%u" k
            sendStrMsg (mailbox.Sender()) "Done, ask for more jobs"
            return! loop()
   
  
        //printfn "[%s]head:%u, tail:%u" mailbox.Self.Path.Name head tail
        for i in head .. tail do
            if(isPerfectSquare(SumConsecSquare (uint64 i) k)) then
                printfn "%u" i 
           
        
        //printfn "\n[%s]: %u-%u" mailbox.Self.Path.Name tail head
        sendStrMsg (mailbox.Sender()) "Done, ask for more jobs"
        return! loop()

    }
    loop()
     

let myBossActor2 paramN paramK (mailbox: Actor<_>) =
    let rec loop lastParamN paramK= actor {
        let! message = mailbox.Receive()
        //printfn "Updated N = %u, k = %u" lastParamN paramK
        if lastParamN = 0 then
            return! loop 0 0

        let n = Math.Max(paramN/100, 1)
        
        if lastParamN <= n then
            sendMsg2 (mailbox.Sender()) (InputMsg2(uint64 1, uint64 lastParamN, uint64 paramK)) 
           //printfn "Msg_done N:%u" n
            return! loop 0 paramK
        else
            sendMsg2 (mailbox.Sender()) (InputMsg2(uint64 (lastParamN-n+1), uint64 lastParamN, uint64 paramK))
            
            let newParamN = lastParamN - n
            //printfn "Boss send msg to %s N:%u" (mailbox.Sender().Path.Name) newParamN
            return! loop newParamN paramK

    }

    for i in 1 .. 8 do
        sendMsg2 (spawnChild myActor (string (i+paramN)))  (InputMsg2(0UL, 0UL, 0UL))
 
    loop paramN paramK


let a = uint64 fsi.CommandLineArgs.[1]
let b = uint64 fsi.CommandLineArgs.[2]
let myBossActorRef2 = spawn system "Boss-Processor-2" (myBossActor2 (int a) (int b))

//printfn "[Boss Actor]%s:Initial Complete" myBossActorRef2.Path.Name
