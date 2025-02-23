local drawableSprite = require("structs.drawable_sprite")

local crate = {}

crate.name = "FactoryHelper/ThrowBox"

crate.fieldInformation = {
    impactParticlesColor = {
        fieldType = "color"
    }
}

crate.placements = {
    {
        name = "wood",
        data = {
            isMetal = false,
            tutorial = false,
            isSpecial = false,
            isCrucial = false,
            canPassThroughSpinners = false,
            textureDirectory = "objects/FactoryHelper/crate",
            overrideParticles = false,
            impactParticlesColor = "9c8d7b"
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
            textureDirectory = "objects/FactoryHelper/crate",
            overrideParticles = false,
            impactParticlesColor = "9c8d7b"
        }
    }
}

--[[function crate.texture(sprite, entity)
    return entity.isMetal and "objects/FactoryHelper/crate/crate_metal0" or "objects/FactoryHelper/crate/crate0"
end]]

function crate.sprite(room, entity)
    local sprites = {}

    local textureDirData = entity.textureDirectory
    local textureDir = (textureDirData == nil or textureDirData == "") and "objects/FactoryHelper/crate" or textureDirData

    local texture = textureDir .. (entity.isMetal and "/crate_metal0" or "/crate0")
    table.insert(sprites, drawableSprite.fromTexture(texture, entity))

    if (entity.isCrucial) then
        local crucialTexture = textureDir .. "/crucial"
        table.insert(sprites, drawableSprite.fromTexture(crucialTexture, entity))
    end

    return sprites
end

crate.justification = {0.0, 0.0}

return crate
