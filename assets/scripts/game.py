from nginz import *
from OpenTK import MathHelper, Vector2, Vector3
from OpenTK.Graphics.OpenGL4 import *
from OpenTK.Input import Key

class MainGame (Game):

    def __init__ (self, conf):
        super (MainGame, self).__init__ (conf)

    def Initialize (self):
        self.LoadContent ()
        self.Mouse.ShouldCenterMouse = True
        self.Mouse.CursorVisible = True
        GL.CullFace (CullFaceMode.Back)
        GL.Enable (EnableCap.CullFace)
        self.camera = FPSCamera (60, self.Resolution, self.Mouse, self.Keyboard)
        self.camera.Camera.SetAbsolutePosition (Vector3 (0, 0, 2))
        self.camera.MouseRotation = Vector2 (MathHelper.DegreesToRadians (180), 0)
        Game.Initialize (self)

    def LoadContent (self):
        self.Content.ContentRoot = "../../assets"
        self.program = self.Content.Load [ShaderProgram] ("passTex")
        self.tex = self.Content.Load [Texture2D] ("testWood.jpg")
        self.box = self.Content.Load [ObjFile] ("box.obj")
        self.model = TexturedModel (self.box, 0, self.program)

    def Update (self, gameTime):
        self.camera.Update (gameTime)
        speed = gameTime.Elapsed.TotalSeconds * 2.0
        if self.Keyboard.IsKeyTyped (Key.Escape):
            Game.Exit (self)
        if self.Keyboard.IsKeyDown (Key.Space):
            self.camera.Camera.SetRelativePosition (Vector3 (0, speed, 0))
        if self.Keyboard.IsKeyDown (Key.LShift):
            self.camera.Camera.SetRelativePosition (Vector3 (0, -speed, 0))
        Game.Update (self, gameTime)

    def Resize (self):
        self.camera.Camera.UpdateCameraMatrix (self.Resolution)
        Game.Resize (self)

    def Draw (self, gameTime):
        GL.ClearColor (.25, .30, .35, 1)
        GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit)
        self.program.Use (self.DrawModel)
        Game.Draw (self, gameTime)

    def DrawModel (self):
        self.model.Draw (self.program, self.camera.Camera, self.tex)