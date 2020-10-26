/*DOS Project for C code*/

#include <stdio.h>
#include <stdlib.h>
#include <math.h>

unsigned long long SumConsecSquare(int n, int k);
int isPerfectSquare(unsigned long long n);

int main(int argc, char *argv[]) {
    printf("\n\n*** DOS Project 1: C code ***\n\n");
    
    int N, k;
    int i;

    if(argc != 3) {
        printf("Wrong Inputs, please enter two integers.\n");
        return 0;
    } else {
        N = atoi(argv[1]);
        k = atoi(argv[2]);
        if(N < 1 || k < 1) {
            printf("Input integers should be greater than 0.\n");
            return 0;
        } else
            printf("Inputs:\nN = %d\nk = %d\n", N, k);
    }
 
    printf("SumConsecSquare from %d to %d is %llu\n", N, k, SumConsecSquare(N, k));
    printf("Is %llu a perfect square: %s\n", SumConsecSquare(N, k), isPerfectSquare(SumConsecSquare(N, k)) ==1?"Yes":"No");

    //Looping thru 1 to N
   
    for(i = 1 ; i <= N ; i++) {
        //Calculate the sums of k consecutive square starts from i
        if(isPerfectSquare(SumConsecSquare(i, k))) {
            printf("%d\n", i);
        }
    }

    return 0;
}

unsigned long long SumConsecSquare(int n, int k) {
    //formula for it
    unsigned long long a = (unsigned long long) n;
    unsigned long long b = (unsigned long long) k;
    unsigned long long temp;
    temp = ( ( a + b - 1 ) * ( a + b ) * ( 2 * a + 2 * b - 1 ) ) / 6;
    b = ( ( a - 1 ) * a * ( 2 * a - 1) ) / 6 ;
    //printf("\n temp = %llu, b = %llu\n", temp, b);
    return temp - b;
    
    //over flow section
    //return (((n+k-1)*(n+k)*((2*n)+(2*k)-1))-((n-1)*n*((2*n)-1)))/6;
}

int isPerfectSquare(unsigned long long n) {
    unsigned long long h = n & 0xF;
    if(h > 9) return 0;
    if(h != 2 && h != 3 && h != 5 && h != 6 && h != 7 && h !=  8) {
        unsigned long long t = (unsigned long long) floor(sqrt((long double)n) + 0.5);
        return t*t == n;
    }
    return 0;
}