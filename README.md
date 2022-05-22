# sharpneat-refactor
Major rewrite/refactor of SharpNEAT.

The code in this repository will eventually be merged into the main sharpneat repo, where it will become SharpNEAT 4.0.

This code base currently targets .NET 6.

There are some UI projects that are based on support for Windows.Forms in dotnet core, however these projects will only run on Windows.
You can however run sharpneat from the console, i.e. without any UI.

The core sharpneat library/agorithm in this repo is completed and tested, so this code is ready for use in research if you wish.
If you do this then you should refer to this project as a SharpNEAT 4.0 pre-release. If you are planning to cite the code then I can 
make a more specific/official release on request.

This refactor/rewrite greatly improves the API, code structure, and performance, and provides a good foundation for future NEAT research.
