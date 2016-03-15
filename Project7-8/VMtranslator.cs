// VMtranslator 
// Takes in VM files, translates them 
// Translates the push and pop arguments

//~~~~~~~~~~~~~ PARSER MODULE ~~~~~~~~~~~~~~

//Call Constructor
	//open .asm file
		//creat array of strings with fule data

	//error message if no file present that works

//Call Parseer function
		//remove all white space and comments 
		
//Bool moreCommands() that constantly checks if there are more commands present
	//while(moreCommands();
		//Read next commmand from input file and make current command
		//untill the boolean function moreCommands() is false
		//meaning there are no more commands

		//While there ARE still commands
			//call commandType() to return the type of command encountered. 
					//examples: C_ARITHEMETIC, C_PUSH, C_POP, C_LABEL, C_GOTO, C_IF, C_FUNCTION, C_RETURN, C_CALL
			//call string arg1()
					//return the first argument of current command
					//not called if the current command is C_RETURN because that means its the end 
			// call int arg2()
					//returns second argument of the current command. 
					//Called only if current command is C_PUSH, C_POP, C_FUNCTION, C_CALL 

//~~~~~~~~~~~~~ CODEWRITER ~~~~~~~~~~~~~~

//Call constructor (ostream out)
		//opens output file/stream and prepares to write to it 	
	//setFileName(string fileName) 
			//iterate through data array?
			//Call writeArithmetic (strong command) to write assembly code translated 
			//from given arithemetic commmand
			//Call WritePushPop(pop/push command, string segment, int index)
				//writes assembly code translated from current command, either C_PUSH or C_POP
		//When all lines have been read out
			//close file


//TEGAN STRALEY & CATIE COOK
//FILE: VMtranslator.cs
//PROJECT: created for project 7 of NAND2Tetris course

//File converts .vm input file to hack assmebly code. The resulting code
//in displayed out to user and also written to a corresponding output file. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace VMtranslator
{
    class Program
    {
        public Program()
        {

        }
   
        private string arithmeticCode1()
        {//pops off the last 2 in the stack 
            return  "@SP\n" +
                    "AM=M-1\n" +
                    "D=M\n" +
                    "A=A-1\n";
        }

        private String arithmeticCode2(String type)
        {//code for the jumps , gt, lt, eq. After each call the arthJumpFlag is incremented to be used properly
            return "@SP\n" +
                    "AM=M-1\n" +
                    "D=M\n" +
                    "A=A-1\n" +
                    "D=M-D\n" +
                    "@FALSE" + arthJumpFlag + "\n" +
                    "D;" + type + "\n" + //type is which jump it was i.e. JNE, JLE, JGE
                    "@SP\n" +
                    "A=M-1\n" +
                    "M=-1\n" +
                    "@CONTINUE" + arthJumpFlag + "\n" +
                    "0;JMP\n" +
                    "(FALSE" + arthJumpFlag + ")\n" +
                    "@SP\n" +
                    "A=M-1\n" +
                    "M=0\n" +
                    "(CONTINUE" + arthJumpFlag + ")\n";
        }

        private string pushCode1(string segment, int index, bool isDirect)
        {
            //if isDirect true = ""
            //if isDirect false = "@" + index + "\n" + "A=D+A\nD=M\n"
            string noPointerCode = (isDirect) ? "" : "@" + index + "\n" + "A=D+A\nD=M\n";

            return "@" + segment + "\n" +
                    "D=M\n" +
                    noPointerCode +
                    "@SP\n" +
                    "A=M\n" +
                    "M=D\n" +
                    "@SP\n" +
                    "M=M+1\n";
        }//end of pushTemplate1

        private string popCode1(string segment, int index, bool isDirect)
        {
            //if isDirect true = "D=A\n"
            //if isDirect false = "D=M\n@" + index + "\nD=D+A\n"
            string noPointerCode = (isDirect) ? "D=A\n" : "D=M\n@" + index + "\nD=D+A\n";

            return "@" + segment + "\n" +
                    noPointerCode +
                    "@R13\n" +
                    "M=D\n" +
                    "@SP\n" +
                    "AM=M-1\n" +
                    "D=M\n" +
                    "@R13\n" +
                    "A=M\n" +
                    "M=D\n";
        }

        //declaration of global variables
        int ARITHMETIC = 0;
        int PUSH = 1;
        int POP = 2;
        int LABEL = 3;
        int GOTO = 4;
        int IF = 5;
        int FUNCTION = 6;
        int RETURN = 7;
        int CALL = 8;
        bool keepGoing;
        int argType;
        string argument1;
        int argument2;
        private int arthJumpFlag = 0;

        void parser(string line, StreamWriter fileOutput, StreamWriter logOutput)
        {
            keepGoing = true;

            argType = 0;
            argument1 = "";
            argument2 = 0;

            //create char array to fill with 
            char[] parsedLine = new char[line.Length];
            int j = 0; //line counter for parsedLine

            //only to take out the comments
            for (int i = 0; i < line.Length && keepGoing == true; i++)
            {
                if (line[i] == '/' && line[i + 1] == '/')
                {
                    //comment has been found! Don't copy any of the rest of the line
                    keepGoing = false; //set keepGoing to false and will fall out of for loop
                }
                else if (line[i] == '\n')
                {
                    keepGoing = false;
                }
                else if (char.IsLetterOrDigit(line[i]) || line[i] == '@' || line[i] == '.' || line[i] == '(' || line[i] == ')' || line[i] == '_' || line[i] == '-' || line[i] == '$' || line[i] == '+' || line[i] == ';' || line[i] == '*' || line[i] == '/' || line[i] == '=' || line[i] == '!' || line[i] == '|' || line[i] == '&' || line[i] == ' ')
                {
                    parsedLine[j] = line[i];
                    j++; //only increment j if [a-zA-Z0-9]*$ has been found in line[i]
                }
                else
                {
                    Console.WriteLine("ERROR: cannot parse line : " + line); //error checking
                    logOutput.WriteLine("ERROR: cannot parse line : " + line);
                    keepGoing = false;
                }
            }//end of for

            int howFull = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (parsedLine[i] == '\0')
                {
                    //don't add to howFull
                }
                else
                {
                    howFull += 1;
                }
            }
            char[] newResult = new char[howFull];
            //copy contents of parsedLine into newResult
            for (int i = 0; i < newResult.Length; i++)
            {
                newResult[i] = parsedLine[i];
            }

            string parsedString = new string(newResult);

            if (newResult.Length != 0) //if parsedLine.Length == 0 means it's an empty array! Just skip this line.
            {
                Console.WriteLine("After taking out comments : " + parsedString);
                logOutput.WriteLine("After taking out comments : " + parsedString);
                string[] splitted = parsedString.Split(' ');

                if(splitted.Length > 3)
                {
                    Console.WriteLine("ERROR: Too many arguments in the line : " + parsedString);
                }
                Console.WriteLine("number of strings in splitted :  " + splitted.Length);
                logOutput.WriteLine("number of strings in splitted :  " + splitted.Length);
                if (splitted[0] == "add" || splitted[0] == "sub" || splitted[0] == "neg" || splitted[0] == "eq" || splitted[0] == "gt" || splitted[0] == "lt" || splitted[0] == "and" || splitted[0] == "or" || splitted[0] == "not")
                {
                    argType = ARITHMETIC;
                    argument1 = splitted[0];
                }
                else if(splitted[0] == "return")
                {
                    argType = RETURN;
                    argument1 = splitted[0];
                }
                else 
                {
                    argument1 = splitted[1];

                    if (splitted[0] == "push")
                    {
                        argType = PUSH;
                    }
                    else if (splitted[0] == "pop")
                    {
                        argType = POP;
                    }
                    else if (splitted[0] == "label")
                    {
                        argType = LABEL;
                    }
                    else if (splitted[0] == "if")
                    {
                        argType = IF;
                    }
                    else if (splitted[0] == "goto")
                    {
                        argType = GOTO;
                    }
                    else if (splitted[0] == "function")
                    {
                        argType = FUNCTION;
                    }
                    else if (splitted[0] == "call")
                    {
                        argType = CALL;
                    }
                    else {
                        Console.WriteLine("ERROR: unknown command type on line : " + parsedString);
                        logOutput.WriteLine("ERROR: unknown command type on line : " + parsedString);
                    }

                    if (argType == PUSH || argType == POP || argType == FUNCTION || argType == CALL){
                        if(int.TryParse(splitted[2], out argument2))
                        {
                            Console.WriteLine("argument2 was an integer.");
                            logOutput.WriteLine("argument2 was an integer.");
                        }
                        else
                        {
                            Console.WriteLine("ERROR : argument2 was not an integer on line : " + parsedString);
                            logOutput.WriteLine("ERROR : argument2 was not an integer on line : " + parsedString);
                        }         
                    }//end of if
                }//end of else

                Console.WriteLine("ArgType : " + argType);
                Console.WriteLine("argument1 : " + argument1);
                Console.WriteLine("argument2 : " + argument2);
                logOutput.WriteLine("ArgType : " + argType);
                logOutput.WriteLine("argument1 : " + argument1);
                logOutput.WriteLine("argument2 : " + argument2);

                if ( argType == ARITHMETIC)
                {
                    if (splitted[0] == "add")
                    {
                        logOutput.WriteLine(arithmeticCode1() + "M=M+D\n");
                        Console.WriteLine(arithmeticCode1() + "M=M+D\n");
                        fileOutput.WriteLine(arithmeticCode1() + "M=M+D\n");
                    }
                    else if (splitted[0] == "sub")
                    {

                        logOutput.WriteLine(arithmeticCode1() + "M=M-D\n");
                        Console.WriteLine(arithmeticCode1() + "M=M-D\n");
                        fileOutput.WriteLine(arithmeticCode1() + "M=M-D\n");

                    }
                    else if (splitted[0] == "and")
                    {

                        logOutput.WriteLine(arithmeticCode1() + "M=M&D\n");
                        Console.WriteLine(arithmeticCode1() + "M=M&D\n");
                        fileOutput.WriteLine(arithmeticCode1() + "M=M&D\n");

                    }
                    else if (splitted[0] == "or")
                    {
                        logOutput.WriteLine(arithmeticCode1() + "M=M|D\n");
                        Console.WriteLine(arithmeticCode1() + "M=M|D\n");
                        fileOutput.WriteLine(arithmeticCode1() + "M=M|D\n");

                    }
                    else if (splitted[0] == "gt")
                    {
                        logOutput.WriteLine(arithmeticCode2("JLE"));
                        Console.WriteLine(arithmeticCode2("JLE"));
                        fileOutput.WriteLine(arithmeticCode2("JLE"));
                        arthJumpFlag++;

                    }
                    else if (splitted[0] == "lt")
                    {
                        logOutput.WriteLine(arithmeticCode2("JGE"));
                        Console.WriteLine(arithmeticCode2("JGE"));
                        fileOutput.WriteLine(arithmeticCode2("JGE"));
                        arthJumpFlag++;

                    }
                    else if (splitted[0] == "eq")
                    {
                        logOutput.WriteLine(arithmeticCode2("JNE"));
                        Console.WriteLine(arithmeticCode2("JNE"));
                        fileOutput.WriteLine(arithmeticCode2("JNE"));
                        arthJumpFlag++;

                    }
                    else if (splitted[0] == "not")
                    {
                        logOutput.WriteLine("@SP\nA=M-1\nM=!M\n");
                        Console.WriteLine("@SP\nA=M-1\nM=!M\n");
                        fileOutput.WriteLine("@SP\nA=M-1\nM=!M\n");
                    }
                    else if (splitted[0] == "neg")
                    {
                        logOutput.WriteLine("D=0\n@SP\nA=M-1\nM=D-M\n");
                        Console.WriteLine("D=0\n@SP\nA=M-1\nM=D-M\n");
                        fileOutput.WriteLine("D=0\n@SP\nA=M-1\nM=D-M\n");
                    }
                    else {
                        Console.WriteLine("ERROR : called writeArithmetic for a non-arithmetic command");
                        logOutput.WriteLine("ERROR : called writeArithmetic for a non-arithmetic command");
                    }
                }
                else if(argType == POP || argType == PUSH)
                {
                    if (argType == PUSH)
                    {

                        if (splitted[1] == "constant")
                        {
                            logOutput.WriteLine("@" + argument2 + "\n" + "D=A\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
                            Console.WriteLine("@" + argument2 + "\n" + "D=A\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
                            fileOutput.WriteLine("@" + argument2 + "\n" + "D=A\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
                        }
                        else if (splitted[1] == "local")
                        {
                            logOutput.WriteLine(pushCode1("LCL", argument2, false));
                            Console.WriteLine(pushCode1("LCL", argument2, false));
                            fileOutput.WriteLine(pushCode1("LCL", argument2, false));

                        }
                        else if (splitted[1] == "argument")
                        {
                            logOutput.WriteLine(pushCode1("ARG", argument2, false));
                            Console.WriteLine(pushCode1("ARG", argument2, false));
                            fileOutput.WriteLine(pushCode1("ARG", argument2, false));
                        }
                        else if (splitted[1] == "this")
                        {
                            logOutput.WriteLine(pushCode1("THIS", argument2, false));
                            Console.WriteLine(pushCode1("THIS", argument2, false));
                            fileOutput.WriteLine(pushCode1("THIS", argument2, false));

                        }
                        else if (splitted[1] == "that")
                        {
                            logOutput.WriteLine(pushCode1("THAT", argument2, false));
                            Console.WriteLine(pushCode1("THAT", argument2, false));
                            fileOutput.WriteLine(pushCode1("THAT", argument2, false));

                        }
                        else if (splitted[1] == "temp")
                        {
                            logOutput.WriteLine(pushCode1("R5", argument2 +5, false));
                            Console.WriteLine(pushCode1("R5", argument2 +5, false));
                            fileOutput.WriteLine(pushCode1("R5", argument2 +5, false));

                        }
                        else if (splitted[1] =="pointer" && argument2 == 0)
                        {
                            logOutput.WriteLine(pushCode1("THIS", argument2 , true));
                            Console.WriteLine(pushCode1("THIS", argument2, true));
                            fileOutput.WriteLine(pushCode1("THIS", argument2 , true));
                        }
                        else if (splitted[1] == "pointer" && argument2 == 1)
                        {
                            logOutput.WriteLine(pushCode1("THAT", argument2, true));
                            Console.WriteLine(pushCode1("THAT", argument2, true));
                            fileOutput.WriteLine(pushCode1("THAT", argument2, true));

                        }
                        else if (splitted[1] == "static")
                        {
                            string mystring = (16 + argument2).ToString();
                            logOutput.WriteLine(pushCode1(mystring, argument2, true));
                            Console.WriteLine(pushCode1(mystring, argument2, true));
                            fileOutput.WriteLine(pushCode1(mystring, argument2, true));
                        }

                    }
                    else if (argType == POP)
                    {

                        if (splitted[1] == "local")
                        {
                            logOutput.WriteLine(popCode1("LCL", argument2, false));
                            Console.WriteLine(popCode1("LCL", argument2, false));
                            fileOutput.WriteLine(popCode1("LCL", argument2, false));
                        }
                        else if (splitted[1] == "argument")
                        {
                            logOutput.WriteLine(popCode1("ARG", argument2, false));
                            Console.WriteLine(popCode1("ARG", argument2, false));
                            fileOutput.WriteLine(popCode1("ARG", argument2, false));
                        }
                        else if (splitted[1] == "this")
                        {
                            logOutput.WriteLine(popCode1("THIS", argument2, false));
                            Console.WriteLine(popCode1("THIS", argument2, false));
                            fileOutput.WriteLine(popCode1("THIS", argument2, false));
                        }
                        else if (splitted[1] == "that")
                        {
                            logOutput.WriteLine(popCode1("THAT", argument2, false));
                            Console.WriteLine(popCode1("THAT", argument2, false));
                            fileOutput.WriteLine(popCode1("THAT", argument2, false));
                        }
                        else if (splitted[1] == "temp")
                        {
                            logOutput.WriteLine(popCode1("R5", argument2 +5, false));
                            Console.WriteLine(popCode1("R5", argument2 +5, false));
                            fileOutput.WriteLine(popCode1("R5", argument2 +5, false));
                        }
                        else if (splitted[1] == "pointer" && argument2 == 0)
                        {
                            logOutput.WriteLine(popCode1("THIS", argument2, true));
                            Console.WriteLine(popCode1("THIS", argument2, true));
                            fileOutput.WriteLine(popCode1("THIS", argument2, true));
                        }
                        else if (splitted[1] == "pointer" && argument2 == 1)
                        {
                            logOutput.WriteLine(popCode1("THAT", argument2, true));
                            Console.WriteLine(popCode1("THAT", argument2, true));
                            fileOutput.WriteLine(popCode1("THAT", argument2, true));
                        }
                        else if (splitted[1] == "static")
                        {
                            string mystring = (16 + argument2).ToString();
                            logOutput.WriteLine(popCode1(mystring, argument2, true));
                            Console.WriteLine(popCode1(mystring, argument2, true));
                            fileOutput.WriteLine(popCode1(mystring, argument2, true));
                        }
                    }
                    else {

                        Console.WriteLine("ERROR : Call writePushPop() for a non-pushpop command line : " + parsedString);
                        logOutput.WriteLine("ERROR : Call writePushPop() for a non-pushpop command line : " + parsedString);

                    }
                }
            }//end of if
        }//end of void parser

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~MAIN~~~~~~~
    public static void Main(string[] args)
    {
            Console.WriteLine("Enter in the .vm file/directory you wish to convert to .asm : ");
            string vmFileName = Console.ReadLine();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(vmFileName);
            Program program = new Program();

            //change .asm to .hack
            char[] asmFileName = new char[vmFileName.Length - 2];
            for (int i = 0; i < vmFileName.Length - 2; i++)
            {
                asmFileName[i] = vmFileName[i]; 
            }

            string asmFileNameString = new string(asmFileName);
            string logFileNameString = new string(asmFileName);
            logFileNameString = string.Concat(asmFileNameString, "log"); //making a .log to fill with same as what we Console.WriteLine();
            asmFileNameString = string.Concat(asmFileNameString, "asm"); //.asm is now .hack
            System.IO.StreamWriter fileOutput = new System.IO.StreamWriter(asmFileNameString);
            System.IO.StreamWriter logOutput = new System.IO.StreamWriter(logFileNameString);

            while ((line = file.ReadLine()) != null)
            { //line by line each loop through
                program.parser(line, fileOutput, logOutput);
            }

            Console.ReadLine();

            file.Close();
            fileOutput.Close();
            logOutput.Close(); //this is a text file to store messages and any error messages 
        }//end of main
    }//end of class Program
}//end of namespace



