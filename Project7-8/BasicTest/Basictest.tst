//BasicTestVME.tst


load BasicTest.asm,
output-file BasicTest.out
compare-to BasticTest.cmp,
output-list RAM[256]%D2.6.2, RAM[300]%D2.6.2, RAM[401]%D2.6.2, 
			RAM[402]%D2.6.2, RAM[3002]%D2.6.2, RAM[2012]%D2.6.2, 
			RAM[3015]%D2.6.2, RAM[11]%D2.6.2;

set RAM[0] 256,
set RAM[1] 300,
set RAM[2] 400,
set RAM[3] 3000,
set RAM[4] 3010,

repeat 600 {
	ticktock;

}

output;