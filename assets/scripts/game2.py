# nginz base
from nginz import *
from nginz.Interop.IronPython import *
from nginz.Common import ScriptEvents

# The game itself.
class MainGame (Game):

    def __init__ (self, conf):
        super (MainGame, self).__init__ (conf)

    # This is just basic initialization code.
    # No need to change anything here
    def Initialize (self):

        # The python script to load
        scriptname = "animation"

        # Set asset path
        self.Content.ContentRoot = "../../assets"

        # Create Python VM to enable live-reloading
        self.vm = PythonVM (self)

        # Call the scripted initialize function
        reload = lambda: self.vm.Call ("initialize", self)
        ScriptEvents.LoadScript += lambda e: self.EnsureContextThread (reload)

        # Load the currently loaded script for live-editing
        script = self.Content.Load [PythonScript] (scriptname)

        # Load the script into the python vm
        self.vm.LoadLive (script)

        Game.Initialize (self)

    def Update (self, gameTime):

        # Call the scripted update function
        self.vm.Call ("update", self, gameTime)
        Game.Update (self, gameTime)

    def Draw (self, gameTime):
        
        # Call the scripted draw function
        self.vm.Call ("draw", self, gameTime)
        Game.Draw (self, gameTime)