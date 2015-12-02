from nginz import *
from nginz.Interop.IronPython import *

class MainGame (Game):

    def __init__ (self, conf):
        super (MainGame, self).__init__ (conf)

    def Initialize (self):

        # Set content root
        self.Content.ContentRoot = "../../assets"

        # Create Python VM to enable live-reloading
        self.vm = PythonVM (self)

        # Load the currently loaded script
        # This may seem paradox, but it makes sense
        script = self.Content.Load [PythonScript] ("game")

        # Load the script into the python vm
        self.vm.LoadLive (script)
        Game.Initialize (self)

    def Update (self, gameTime):

        # Call the scripted update function
        self.vm ["scriptedUpdate"] (gameTime)
        Game.Update (self, gameTime)

    def Draw (self, gameTime):
        
        # Call the scripted draw function
        self.vm ["scriptedDraw"] (gameTime)
        Game.Draw (self, gameTime)

def scriptedUpdate (gameTime):
    print ("Updating!")

def scriptedDraw (gameTime):
    print ("Drawing!")