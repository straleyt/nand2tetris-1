//StaticTestVME.tst

load StaticTest.asm,
output-file StaticTest.out,
compare-to StaticTest.cmp,
output-list RAM[256]%D1.6.1;
	

set sp 256,

repeat 11 {
	vmstep;
}

output;
