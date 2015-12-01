speed = 5
forward = true
backwardFinished = false

function load ()
    game:invoke (function ()
        tex = content:loadTexture ("nginz.png")
        x1 = -tex:width ()
        x2 = game:width ()
        y1 = game.height ()
        y2 = -tex.height ()
        centerx = (game:width () / 2) - (tex:width () / 2)
        centery = (game:height () / 2) - (tex:height () / 2)
    end)
end

function unload ()
    spriteBatch:flush ()
end

function update ()
    if forward then
        if x1 < centerx then x1 = x1 + speed end
        if x2 > centerx then x2 = x2 - speed end
        if y1 > centery then y1 = y1 - speed * 0.6 end
        if y2 < centery then y2 = y2 + speed * 0.6 end
        if x1 >= centerx and x2 <= centerx and
           y1 <= centery and y2 >= centery then
           forward = false
        end
    else
        if x1 > -tex:width () then x1 = x1 - speed end
        if x2 < game:width () then x2 = x2 + speed end
        if x1 <= -tex:width () and x2 >= game:width () then
            backwardFinished = true
        end
        if backwardFinished then
            if y1 < game:height () then y1 = y1 + speed * 0.6 end
            if y2 > -tex:height () then y2 = y2 - speed * 0.6 end
            if y1 >= game.height () and y2 <= -tex.height () then
                forward = true
                backwardFinished = false
            end
        end
    end
end

function draw ()
    white = color.new (1, 1, 1, 1)
    gl:clearColor (0.4, 0, 1, 1.0)
    gl:clear ("ColorBufferBit")
    spriteBatch:begin ()
    spriteBatch:draw (tex, vec2.new (x1, centery), white)
    spriteBatch:draw (tex, vec2.new (centerx, y1), white)
    spriteBatch:draw (tex, vec2.new (x2, centery), white)
    spriteBatch:draw (tex, vec2.new (centerx, y2), white)
    spriteBatch:flush ()
end