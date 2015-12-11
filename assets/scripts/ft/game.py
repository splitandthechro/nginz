from OpenTK.Graphics.OpenGL4 import GL, ClearBufferMask
from OpenTK.Graphics import Color4
from OpenTK import Vector2
from nginz import Texture2D, Animator, SpriteSheet2D, TextureConfiguration

tex_fireball = None
game = None
content = None
animator = None
sheet = None

def initialize (_game):
    global game
    game = _game
    loadcontent (game, game.Content)

def loadcontent (_game, _content):
    global game, content
    game = _game
    content = _content
    game.EnsureContextThread (internalloadcontent)

def internalloadcontent ():
    global game, content, sheet, animator, tex_fireball
    tex_fireball = content.Load [Texture2D] ("ft/fireball.png")
    sheet = SpriteSheet2D (tex_fireball, 8, 1)
    animator = Animator (sheet, 8)
    animator.DurationInMilliseconds = 750
    animator.Scale = Vector2 (2, 2)
    animator.Speed = 1
    animator.Position = Vector2 (animator.Position.X, game.Bounds.Height)

def update (game, time):
    animator.Update (time)
    animator.Position = Vector2 (animator.Position.X, animator.Position.Y - (750 * time.Elapsed.TotalSeconds))
    if (animator.Position.Y < -(sheet [0].Height)):
        animator.Position = Vector2 (animator.Position.X, game.Bounds.Height)
    pass

def draw (game, time):
    GL.ClearColor (Color4.Black)
    GL.Clear (ClearBufferMask.ColorBufferBit)
    pass

def draw2d (game, time, batch):
    animator.Draw (time, batch)
    pass