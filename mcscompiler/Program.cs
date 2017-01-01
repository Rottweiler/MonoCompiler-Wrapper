using System;

namespace mcscompiler
{
    class Program
    {
        static MonoCompiler compiler = new MonoCompiler()
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

        static void Main(string[] args)
        {
            using (VirtualSourceFile vcs = new VirtualSourceFile(Properties.Resources.hello_world))
            {
                var result = compiler.Compile(vcs.Location, "hello_world.exe");
                foreach(Error error in result.Errors )
                {
                    Console.WriteLine(error.Message);
                }
            }

            Console.ReadLine();
        }
    }
}
