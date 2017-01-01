<a href="#"><img src="https://upload.wikimedia.org/wikipedia/en/thumb/b/b4/Mono_project_logo.svg/860px-Mono_project_logo.svg.png" align="left" height="48" width="48" ></a>

#MonoCompiler-Wrapper
C#.NET Wrapper for the (mcs.exe) Mono Compiler - Compile Mono projects from your application

# Usage
```c#
MonoCompiler compiler = new MonoCompiler()
{
    Compiler_path = "mcs.exe",
    Platform = Platform.anycpu,
    SDK_version = 2,
    Target = Target.exe,
    Unsafe = true,
    Optimize = true,
    Language_version = LangVersion.Experimental,
    Reference_mscorlib = true
    //Icon = "path.ico"
};

var result = compiler.Compile("hello_world.cs", "hello_world.exe");
foreach(Error error in result.Errors )
{
    Console.WriteLine(error.Message);
}
