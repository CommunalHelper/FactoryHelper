local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local utils = require("utils")

local boomBoxZip = {}

boomBoxZip.name = "FactoryHelper/BoomBoxZip"
boomBoxZip.nodeVisibility = "never"
boomBoxZip.nodeLimits = {1, 1}
boomBoxZip.canResize = {false, false}

-- width added as a hack to fix default node placements
boomBoxZip.ignoredFields = {"_name", "_id", "width"}

boomBoxZip.fieldInformation = {
    initialDelay = {
        minimumValue = 0.0
    },
    ropeColor = {
        fieldType = "color"
    },
    ropeLightColor = {
        fieldType = "color"
    },
    sparkParticleColor = {
        fieldType = "color"
    }
}

boomBoxZip.placements = {
    name = "active",
    data = {
        width = 24,
        activationId = "",
        initialDelay = 0.0,
        startActive = true,
        spriteDir = "",
        ropeColor = "4d3c22",
        ropeLightColor = "766c49",
        sparkParticleColor = "fff538"
    }
}

local function addNodeSprites(sprites, entity, centerX, centerY, centerNodeX, centerNodeY, spriteDir)
    local ropeColor = utils.getColor(entity.ropeColor) or {77 / 255, 60 / 255, 34 / 255}

    local cogSprite = drawableSprite.fromTexture(spriteDir .. "/zipMoverCog", entity) or drawableSprite.fromTexture("objects/zipmover/cog", entity)
    cogSprite:setPosition(centerNodeX, centerNodeY)
    cogSprite:setJustification(0.5, 0.5)

    local points = {centerX, centerY, centerNodeX, centerNodeY}
    local leftLine = drawableLine.fromPoints(points, ropeColor, 1)
    local rightLine = drawableLine.fromPoints(points, ropeColor, 1)

    leftLine:setOffset(0, 4.5)
    rightLine:setOffset(0, -4.5)

    leftLine.depth = 5000
    rightLine.depth = 5000

    for _, sprite in ipairs(leftLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end

    for _, sprite in ipairs(rightLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end

    table.insert(sprites, cogSprite)
end

function boomBoxZip.sprite(room, entity)
    local sprites = {}

    local x, y = entity.x or 0, entity.y or 0
    local nodes = entity.nodes or {{x = 0, y = 0}}
    local spriteDir = (entity.spriteDir or "") == "" and "objects/FactoryHelper/boomBox" or entity.spriteDir

    -- Draw cog + rope
    local halfWidth = 12
    local centerX = x + halfWidth
    local centerY = y + halfWidth
    local centerNodeX = nodes[1].x + halfWidth
    local centerNodeY = nodes[1].y + halfWidth
    addNodeSprites(sprites, entity, centerX, centerY, centerNodeX, centerNodeY, spriteDir)
    
    -- Draw boom box
    local boomBoxTex = spriteDir .. (entity.startActive and "/active00" or "/idle00")
    local boomBoxSprite = drawableSprite.fromTexture(boomBoxTex, entity)
    boomBoxSprite:setPosition(centerX, centerY)
    boomBoxSprite:setJustification(0.5, 0.5)
    table.insert(sprites, boomBoxSprite)

    return sprites
end

function boomBoxZip.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local nodes = entity.nodes or {{x = 0, y = 0}}

    local halfWidth = 12
    local centerX = x + halfWidth
    local centerY = y + halfWidth
    local centerNodeX = nodes[1].x + halfWidth
    local centerNodeY = nodes[1].y + halfWidth

    local cogTexture = "objects/zipmover/cog"
    local cogSprite = drawableSprite.fromTexture(cogTexture, entity)
    local cogWidth, cogHeight = cogSprite.meta.width, cogSprite.meta.height

    local mainRectangle = utils.rectangle(x, y, 24, 24)
    local nodeRectangle = utils.rectangle(centerNodeX - math.floor(cogWidth / 2), centerNodeY - math.floor(cogHeight / 2), cogWidth, cogHeight)

    return mainRectangle, {nodeRectangle}
end

return boomBoxZip
