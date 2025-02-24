local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")
local fakeTilesHelper = require("helpers.fake_tiles")

local crateSpawner = {}

crateSpawner.name = "FactoryHelper/ThrowBoxSpawner"

crateSpawner.fieldInformation = function()
    return {
        delay = {
            minimumValue = 0.0
        },
        maximum = {
            fieldType = "integer",
            minimumValue = 0
        },
        impactParticlesColor = {
            fieldType = "color"
        },
        debrisFromTiletype = {
            options = fakeTilesHelper.getTilesOptions(),
            editable = false
        },
        metalDebrisFromTiletype = {
            options = fakeTilesHelper.getTilesOptions(),
            editable = false
        }
    }
end

crateSpawner.placements = {
    {
        name = "crate_spawner",
        data = {
            delay = 1.0,
            maximum = 0,
            activationId = "",
            isMetal = false,
            isRandom = false,
            fromTop = true,
            tutorial = false,
            startActive = true,
            canPassThroughSpinners = false,
            overrideTextures = false,
            woodenCrateTexturePath = "objects/FactoryHelper/crate/crate0",
            metalCrateTexturePath = "objects/FactoryHelper/crate/crate_metal0",
            overrideParticles = false,
            impactParticlesColor = "9c8d7b",
            overrideDebris = false,
            woodenDebrisFromTiletype = '9',
            metalDebrisFromTiletype = '8'
        }
    }
}

local normalColor = {1.0, 1.0, 1.0, 0.2}
local randomColor = {1.0, 0.5, 1.0, 0.2}

function crateSpawner.sprite(room, entity)
    local sprites = {}

    local texture = ""
    if (entity.overrideTextures) then
        texture = entity.isMetal and (entity.metalCrateTexturePath or "") or (entity.woodenCrateTexturePath or "")
    else
        texture = entity.isMetal and "objects/FactoryHelper/crate/crate_metal0" or "objects/FactoryHelper/crate/crate0"
    end

    local crateSprite = drawableSprite.fromTexture(texture, entity)
    crateSprite.color = {1.0, 1.0, 1.0, 0.5}

    local x, y = entity.x or 0, entity.y or 0

    local baseColor = entity.isRandom and randomColor or normalColor
    local edgeColor = {baseColor[1], baseColor[2], baseColor[3], 0.5}
    local rect = drawableRectangle.fromRectangle("bordered", x - 8, y - 8, 16, 16, baseColor, edgeColor)

    table.insert(sprites, crateSprite)
    table.insert(sprites, rect)

    return sprites
end

return crateSpawner
