# PlatformThingie
Want to dive into dynamically loading assemblies at runtime, and what that can lead to in terms of design


Notes:
- copy the dll and pdb from defaults to where web platform outputs
- a dotnet process is created when calling the app from terminal, to attach debbuger to process
- debugging startup stuff will still require pasting the dll's in the output directory
- feasable to make scripts for different app configurations, yeey modularity