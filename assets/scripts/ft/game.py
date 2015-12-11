from OpenTK.Graphics.OpenGL4 import GL, ClearBufferMask
from OpenTK.Graphics import Color4
from OpenTK.Input import Key, MouseButton
from OpenTK import Vector2
from nginz import Texture2D, Animator, SpriteSheet2D, TextureConfiguration
from System.Drawing import Rectangle

walking = False
fireball_size = 1

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
    global game, content
    global tex_fireball, animator_fireball, sheet_fireball
    global tex_character_walk, animator_character_walk, sheet_character_walk
    tex_fireball = content.Load [Texture2D] ("ft/fireball.png", TextureConfiguration.LinearMipmap)
    tex_character_walk = content.Load [Texture2D] ("ft/char_walk.png", TextureConfiguration.LinearMipmap)
    sheet_fireball = SpriteSheet2D (tex_fireball, 8, 1)
    sheet_character_walk = SpriteSheet2D (tex_character_walk, 3, 1)
    animator_fireball = Animator (sheet_fireball, 8)
    animator_fireball.DurationInMilliseconds = 750
    animator_fireball.Scale = Vector2 (1, 1)
    animator_fireball.Speed = 1
    animator_fireball.Position = Vector2 (animator_fireball.Position.X, game.Bounds.Height)
    animator_fireball.Origin = Vector2 (animator_fireball.Sheet [0].Width / 2, animator_fireball.Sheet [0].Height / 2)
    animator_character_walk = Animator (sheet_character_walk, 2, 1)
    animator_character_walk.DurationInMilliseconds = 500
    animator_character_walk.Position = Vector2 ((game.Bounds.Width / 2) - (tex_character_walk.Width / 2), (game.Bounds.Height / 2) - (tex_character_walk.Height / 2))

def update (game, time):
    global walking, fireball_size
    speed = 100 * time.Elapsed.TotalSeconds
    if game.Keyboard.IsAnyKeyDown (Key.W, Key.S, Key.A, Key.D):
        walking = True
    else:
        walking = False
    if game.Keyboard.IsKeyDown (Key.W):
        animator_character_walk.Position = Vector2 (animator_character_walk.Position.X, animator_character_walk.Position.Y - speed)
    if game.Keyboard.IsKeyDown (Key.S):
        animator_character_walk.Position = Vector2 (animator_character_walk.Position.X, animator_character_walk.Position.Y + speed)
    if game.Keyboard.IsKeyDown (Key.A):
        animator_character_walk.Position = Vector2 (animator_character_walk.Position.X - speed, animator_character_walk.Position.Y)
    if game.Keyboard.IsKeyDown (Key.D):
        animator_character_walk.Position = Vector2 (animator_character_walk.Position.X + speed, animator_character_walk.Position.Y)
    if game.Mouse.IsButtonDown (MouseButton.Left) and game.Mouse.IsInsideWindow ():
        fireball_size += time.Elapsed.TotalSeconds
        animator_fireball.Scale = Vector2 (fireball_size, fireball_size)
        animator_fireball.Position = Vector2 (animator_character_walk.Position.X + (animator_character_walk.Sheet [0].Width / 2) + (animator_fireball.Sheet [0].Width / 2) + 18, animator_character_walk.Position.Y + (animator_character_walk.Sheet [0].Height / 2) - (tex_fireball.Height / 2))
    else:
    	animator_fireball.Position = Vector2 (animator_fireball.Position.X, animator_fireball.Position.Y - (750 * time.Elapsed.TotalSeconds))
    
    animator_fireball.Update (time)
    #if (animator_fireball.Position.Y < -(sheet_fireball [0].Height)):
    #    animator_fireball.Position = Vector2 (animator_fireball.Position.X, game.Bounds.Height)
    animator_character_walk.Update (time)

def draw (game, time):
    GL.ClearColor (Color4.Black)
    GL.Clear (ClearBufferMask.ColorBufferBit)
    pass

def draw2d (game, time, batch):
    animator_fireball.Draw (time, batch)
    if walking:
        animator_character_walk.Draw (time, batch)
    else:
        batch.Draw (animator_character_walk.Sheet.Texture, Rectangle (animator_character_walk.Sheet [0].X, animator_character_walk.Sheet [0].Y, animator_character_walk.Sheet [0].Width, animator_character_walk.Sheet [0].Height), animator_character_walk.Position, Color4.White)