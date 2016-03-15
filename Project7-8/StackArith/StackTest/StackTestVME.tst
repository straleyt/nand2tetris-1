//StackTestVME.tst

load StackTest.vm,
output-file StackTest.out,
compare-to StackTest.cmp,
output-list RAM[0]%D2.6.2
	RAM[258]%D2.6.2 RAM[259]%D2.6.2 RAM[260]%D2.6.2;

set RAM[0] 256,

repeat 38 {
	vmstep;
}

output;
output-list RAM[261]%D2.6.2 RAM[262]%D2.6.2 RAM[263]%D2.6.2 RAM[264]%D2.6.2 RAM[265}%D2.6.2;
output;
