namespace Patches.CLI;

public partial class PatchesCLI
{
    public void HelpScreen()
    {
        string helpScreen =
@"Patches
        
    Commands:
    
    'add'   Add a new Module
    'list'  List all Modules
    
    'q' to Quit";

        UI.WriteLine(helpScreen);
    }
}
