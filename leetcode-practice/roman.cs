public class Solution {
    /*
    behavior notes: a smaller number PRECEEDING a bigger number causes a subtraction of the smaller number
    from the bigger number.
    I can toggle modes or compare against using size testing
    */
    
    enum numeral {I=1, V=5, X=10, L=50, C=100, D=500, M=1000};
    public int RomanToInt(string s) {
        int test = 0;     
        char[] testInput = s.ToCharArray();
        int max = Lookup(testInput[0]);
        for(int i = 0; i < testInput.Length; i++){
            int val = Lookup(testInput[i]); 
                if(val == max){
                    test += max;
                }
               if(val < max){
                   test += val;
                   max = val;
               }
            
                else if (val > max){
                    test += val - max - max;
                    max = val;
                }
        }
        return test;
    }
    
    public int Lookup(char number){
        if(number == 'I'){
            return (int) numeral.I;
        }
        
        else if(number == 'V'){
            return (int) numeral.V;
        }
        
        else if(number == 'X'){
            return (int) numeral.X;
        }
        
        else if(number == 'L'){
            return (int) numeral.L;
        }
        
        else if (number == 'C'){
            return (int) numeral.C;
        }
        else if (number == 'D'){
            return (int) numeral.D;
        }
        
        else{
            return (int) numeral.M;
        }
        
        
    }
} 
