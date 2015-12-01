from nginz import Texture2D
from OpenTK import Vector2
from OpenTK.Graphics import Color4
from OpenTK.Graphics.OpenGL4 import *

speed = 10
forward = True
backwardFinished = False

width = game.Resolution.Width
height = game.Resolution.Height

def init ():
    global tex, x1, x2, y1, y2, centerx, centery
    tex = game.Content.Load[Texture2D]("nginz.png")
    x1 = -tex.Width
    x2 = width
    y1 = height
    y2 = -tex.Height
    centerx = (width / 2.0) - (tex.Width / 2.0)
    centery = (height / 2.0) - (tex.Height / 2.0)

def update ():
    global tex, forward, backwardFinished
    global x1, x2, y1, y2, centerx, centery
    if forward:
        if x1 < centerx: x1 += speed
        if x2 > centerx: x2 -= speed
        if y1 > centery: y1 -= speed * 0.6
        if y2 < centery: y2 += speed * 0.6
        if x1 >= centerx and x2 <= centerx and \
           y1 <= centery and y2 >= centery:
           forward = False
    else:
        if x1 > -tex.Width: x1 -= speed
        if x2 < width: x2 += speed
        if x1 <= -tex.Width and x2 >= width:
            backwardFinished = True
        if backwardFinished:
            if y1 < height: y1 += speed * 0.6
            if y2 > -tex.Height: y2 -= speed * 0.6
            if y1 >= height and y2 <= -tex.Height:
                forward = True
                backwardFinished = False

def draw ():
    GL.ClearColor (0.3, 0, 0.8, 1)
    GL.Clear (ClearBufferMask.ColorBufferBit)
    game.SpriteBatch.Begin ()
    game.SpriteBatch.Draw (tex, Vector2 (x1, centery), Color4.White)
    game.SpriteBatch.Draw (tex, Vector2 (centerx, y1), Color4.White)
    game.SpriteBatch.Draw (tex, Vector2 (x2, centery), Color4.White)
    game.SpriteBatch.Draw (tex, Vector2 (centerx, y2), Color4.White)
    game.SpriteBatch.End ()