local drawableSprite = require("structs.drawable_sprite")
local fakeTilesHelper = require("helpers.fake_tiles")

local crate = {}

crate.name = "FactoryHelper/ThrowBox"

crate.fieldInformation = function()
    return {
        impactParticlesColor = {
            fieldType = "color"
        },
        debrisFromTiletype = {
            options = fakeTilesHelper.getTilesOptions(),
            editable = false
        }
    }
end

crate.placements = {
    {
        name = "wood",
        data = {
            isMetal = false,
            tutorial = false,
            isSpecial = false,
            isCrucial = false,
            canPassThroughSpinners = false,
            overrideTextures = false,
            crateTexturePath = "objects/FactoryHelper/crate/crate0",
            crucialTexturePath = "objects/FactoryHelper/crate/crucial",
            overrideParticles = false,
            impactParticlesColor = "9c8d7b",
            overrideDebris = false,
            debrisFromTiletype = '9'
        }
    },
    {
        name = "metal",
        data = {
            isMetal = true,
            tutorial = false,
            isSpecial = false,
            isCrucial = false,
            canPassThroughSpinners = false,
            overrideTextures = false,
            crateTexturePath = "objects/FactoryHelper/crate/crate_metal0",
            crucialTexturePath = "objects/FactoryHelper/crate/crucial",
            overrideParticles = false,
            impactParticlesColor = "9c8d7b",
            overrideDebris = false,
            debrisFromTiletype = '8'
        }
    }
}

function crate.sprite(room, entity)
    local sprites = {}

    if (entity.overrideTextures) then
        local crateSprite = entity.crateTexturePath or ""
        table.insert(sprites, drawableSprite.fromTexture(crateSprite, entity))

        if (entity.isCrucial) then
            local crucialTexture = entity.crucialTexturePath or ""
            table.insert(sprites, drawableSprite.fromTexture(crucialTexture, entity))
        end
    else
        local crateSprite = entity.isMetal and "objects/FactoryHelper/crate/crate_metal0" or "objects/FactoryHelper/crate/crate0"
        table.insert(sprites, drawableSprite.fromTexture(crateSprite, entity))

        if (entity.isCrucial) then
            local crucialTexture = "objects/FactoryHelper/crate/crucial"
            table.insert(sprites, drawableSprite.fromTexture(crucialTexture, entity))
        end
    end

    return sprites
end

crate.justification = {0.0, 0.0}

return crate
