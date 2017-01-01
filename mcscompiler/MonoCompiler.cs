using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace mcscompiler
{
    enum Platform
    {
        anycpu,
        anycpu32bitpreferred,
        arm,
        x86,
        x64,
        itanium
    }

    enum Target
    {
        exe,
        winexe,
        library,
        module
    }

    /// <summary>
    /// ISO-1, ISO-2, 3, 4, 5, Default or Experimental
    /// </summary>
    enum LangVersion
    {
        Default,
        ISO_1,
        ISO_2,
        _3,
        _4,
        _5,
        Experimental
    }

    enum Version
    {
        _2,
        _3,
        _35,
        _4,
        _45
    }

    /// <summary>
    /// Mono Compiler Wrapper (mcs.exe)
    /// </summary>
    class MonoCompiler
    {
        /// <summary>
        /// Path to the mcs compiler executable
        /// </summary>
        public string Compiler_path { get; set; }

        /// <summary>
        /// Specifies the target platform of the output assembly ARCH can be one of: anycpu, anycpu32bitpreferred, arm, x86, x64 or itanium. The default is anycpu.
        /// </summary>
        public Platform Platform { get; set; }

        /// <summary>
        /// Specifies the path to the Win32 icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Specifies unsafe mode
        /// </summary>
        public bool Unsafe { get; set; }

        /// <summary>
        /// Specifies optimization
        /// </summary>
        public bool Optimize { get; set; }

        /// <summary>
        /// Specifies the SDK version (2, 3, 4, 4.5)
        /// </summary>
        public double SDK_version { get; set; }

        /// <summary>
        /// ISO-1, ISO-2, 3, 4, 5, Default or Experimental
        /// </summary>
        public LangVersion Language_version { get; set; }

        /// <summary>
        /// Specify whether to reference mscorlib or not
        /// </summary>
        public bool Reference_mscorlib { get; set; }

        /// <summary>
        /// Specifies the format of the output assembly (exe, winexe, library, module)
        /// </summary>
        public Target Target { get; set; }

        public MonoCompiler()
        {

        }

        private string TranslateLanguageVersion(LangVersion lang)
        {
            switch (lang)
            {
                case LangVersion.Default:
                    return "Default";
                case LangVersion.ISO_1:
                    return "ISO-1";
                case LangVersion.ISO_2:
                    return "ISO-2";
                case LangVersion._3:
                    return "3";
                case LangVersion._4:
                    return "4";
                case LangVersion._5:
                    return "5";
                case LangVersion.Experimental:
                    return "Experimental";
                default:
                    return "Default";
            }
        }

        /// <summary>
        /// Compile the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="output_assembly"></param>
        /// <returns></returns>
        public ErrorList Compile(string source, string output_assembly = "", string output_documentation = "")
        {
            StringBuilder arguments = new StringBuilder();
            List<Error> errors = new List<Error>();

            if (!File.Exists(Compiler_path))
                throw new FileNotFoundException("Compiler executable not found.");

            //build arguments
            arguments.Append(Quote(source) + " ");
            arguments.Append(" -platform:" + Platform);
            arguments.Append(" -target:" + Target);
            arguments.Append(" -sdk:" + SDK_version);
            arguments.Append(" --runtime:" + "v" + SDK_version);
            arguments.Append(" -unsafe" + (Unsafe ? "+" : "-"));
            arguments.Append(" -optimize" + (Optimize ? "+" : "-"));
            arguments.Append(" -langversion:" + TranslateLanguageVersion(Language_version));
            arguments.Append(" -nostdlib" + (!Reference_mscorlib ? "+" : "-")); //-nostdlib[+|-]       Does not reference mscorlib.dll library

            if (!string.IsNullOrEmpty(output_assembly))
                arguments.Append(" -out:" + Quote(output_assembly));

            if (!string.IsNullOrEmpty(Icon))
                arguments.Append(" -win32icon:" + Quote(Icon));

            if (!string.IsNullOrEmpty(output_documentation))
                arguments.Append(" -doc:" + Quote(output_documentation));

            Process compiler_process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Compiler_path,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = arguments.ToString(),
                    CreateNoWindow = true
                }
            };

            compiler_process.Start();
            compiler_process.WaitForExit();

            //record errors
            while (!compiler_process.StandardOutput.EndOfStream)
            {
                string line = compiler_process.StandardOutput.ReadLine();
                if (!string.IsNullOrEmpty(line) && line.ToLower().Contains("error"))
                    errors.Add(new Error()
                    {
                        Message = line
                    });
            }

            return new ErrorList(errors);
        }

        /// <summary>
        /// Apply quotations
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string Quote(string text)
        {
            return "\"" + text + "\"";
        }
    }

    class Error
    {
        public int Line { get; set; }
        public string Message { get; set; }
    }

    class ErrorList
    {
        public List<Error> Errors { get; set; }
        public ErrorList(List<Error> errors)
        {
            Errors = errors;
        }
    }
}

/*
Mono C# compiler, Copyright 2001-2011 Novell, Inc., Copyright 2011-2012 Xamarin, Inc
mcs [options] source-files
   --about              About the Mono C# compiler
   -addmodule:M1[,Mn]   Adds the module to the generated assembly
   -checked[+|-]        Sets default aritmetic overflow context
   -clscheck[+|-]       Disables CLS Compliance verifications
   -codepage:ID         Sets code page to the one in ID (number, utf8, reset)
   -define:S1[;S2]      Defines one or more conditional symbols (short: -d)
   -debug[+|-], -g      Generate debugging information
   -delaysign[+|-]      Only insert the public key into the assembly (no signing)
   -doc:FILE            Process documentation comments to XML file
   -fullpaths           Any issued error or warning uses absolute file path
   -help                Lists all compiler options (short: -?)
   -keycontainer:NAME   The key pair container used to sign the output assembly
   -keyfile:FILE        The key file used to strongname the ouput assembly
   -langversion:TEXT    Specifies language version: ISO-1, ISO-2, 3, 4, 5, Default or Experimental
   -lib:PATH1[,PATHn]   Specifies the location of referenced assemblies
   -main:CLASS          Specifies the class with the Main method (short: -m)
   -noconfig            Disables implicitly referenced assemblies
   -nostdlib[+|-]       Does not reference mscorlib.dll library
   -nowarn:W1[,Wn]      Suppress one or more compiler warnings
   -optimize[+|-]       Enables advanced compiler optimizations (short: -o)
   -out:FILE            Specifies output assembly name
   -pkg:P1[,Pn]         References packages P1..Pn
   -platform:ARCH       Specifies the target platform of the output assembly
                        ARCH can be one of: anycpu, anycpu32bitpreferred, arm,
                        x86, x64 or itanium. The default is anycpu.
   -recurse:SPEC        Recursively compiles files according to SPEC pattern
   -reference:A1[,An]   Imports metadata from the specified assembly (short: -r)
   -reference:ALIAS=A   Imports metadata using specified extern alias (short: -r)
   -sdk:VERSION         Specifies SDK version of referenced assemblies
                        VERSION can be one of: 2, 4, 4.5 (default) or a custom value
   -target:KIND         Specifies the format of the output assembly (short: -t)
                        KIND can be one of: exe, winexe, library, module
   -unsafe[+|-]         Allows to compile code which uses unsafe keyword
   -warnaserror[+|-]    Treats all warnings as errors
   -warnaserror[+|-]:W1[,Wn] Treats one or more compiler warnings as errors
   -warn:0-4            Sets warning level, the default is 4 (short -w:)
   -helpinternal        Shows internal and advanced compiler options

Resources:
   -linkresource:FILE[,ID] Links FILE as a resource (short: -linkres)
   -resource:FILE[,ID]     Embed FILE as a resource (short: -res)
   -win32res:FILE          Specifies Win32 resource file (.res)
   -win32icon:FILE         Use this icon for the output
   @file                   Read response file for more options

Options can be of the form -option or /option
*/
