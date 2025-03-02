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
            canPassThroughSpinners = false
        }
    },
    {
        name = "metal",
        data = {
            isMetal = true,
            tutorial = false,
            isSpecial = false,
            isCrucial = false,
            canPassThroughSpinners = false
        }
    },
    {
        name = "reskin_wood",
        data = {
            isMetal = false,
            tutorial = false,
            isSpecial = false,
            isCrucial = false,
            canPassThroughSpinners = false,
            overrideTextures = true,
            crateTexturePath = "objects/FactoryHelper/crate/crate0",
            crucialTexturePath = "objects/FactoryHelper/crate/crucial",
            overrideParticles = false,
            impactParticlesColor = "9c8d7b",
            overrideDebris = false,
            debrisFromTiletype = '9'
        }
    },
    {
        name = "reskin_metal",
        data = {
            isMetal = true,
            tutorial = false,
            isSpecial = false,
            isCrucial = false,
            canPassThroughSpinners = false,
            overrideTextures = true,
            crateTexturePath = "objects/FactoryHelper/crate/crate_metal0",
            crucialTexturePath = "objects/FactoryHelper/crate/crucial",
            overrideParticles = false,
            impactParticlesColor = "9c8d7b",
            overrideDebris = false,
            debrisFromTiletype = '8'
        }
    }
}

local justification = {0.0, 0.0}

function crate.sprite(room, entity)
    local sprites = {}

    if (entity.overrideTextures) then
        local crateSprite = drawableSprite.fromTexture(entity.crateTexturePath or "", entity)
        crateSprite:setJustification(justification)
        table.insert(sprites, crateSprite)

        if (entity.isCrucial) then
            local crucialSprite = drawableSprite.fromTexture(entity.crucialTexturePath or "", entity)
            crucialSprite:setJustification(justification)
            table.insert(sprites, crucialSprite)
        end
    else
        local crateSprite = drawableSprite.fromTexture(entity.isMetal and "objects/FactoryHelper/crate/crate_metal0" or "objects/FactoryHelper/crate/crate0", entity)
        crateSprite:setJustification(justification)
        table.insert(sprites, crateSprite)

        if (entity.isCrucial) then
            local crucialSprite = drawableSprite.fromTexture("objects/FactoryHelper/crate/crucial", entity)
            crucialSprite:setJustification(justification)
            table.insert(sprites, crucialSprite)
        end
    end

    return sprites
end

return crate
