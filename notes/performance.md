# Performance Testing/Profiling Notes

* Ensure server GC is enabled.
* Bear in mind the later versions of dotnet core use tiered compilation, and this is enabled by default from dotnet core 3.0 onwards (see https://docs.microsoft.com/en-us/dotnet/core/run-time-config/compilation)
* Consider spectre/meltdown mitigations. This will hopefully be increasingly irrelevant over time; the initial mitigations had significant impact on some specific subroutines (but not sure about the overall effect on SharpNEAT).
