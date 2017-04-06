# Unity3d-ProjectBuilder
this tool simplifies building a few small or medium projects inside a bigger one with ease.

this is done by a configuration file (scriptableobject) to input the name of the sub project , and the scenes that are involved in it.

in an editor window accessible from Windows tab, you can change the Application.Version string, with increment buttons.
and specify the platform, and additional build options you would like.

each sub project that was setup in the configuration ,will have its own button.
this will be clicked and the build will proceed to dedicated folders inside a Builds folder in Application.DataPath 
