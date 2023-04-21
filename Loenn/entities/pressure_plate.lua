local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local pressurePlate = {}

pressurePlate.name = "FactoryHelper/PressurePlate"

pressurePlate.placements = {
    name = "pressure_plate",
    data = {
        activationIds = ""
        multiplayerMode = false
    }
}

function pressurePlate.sprite(room, entity)
    local button = drawableSprite.fromTexture("objects/FactoryHelper/pressurePlate/plate_button0", entity)
    button:setJustification(0.0, 0.0)
    button:addPosition(-2, -2)

    local case = drawableSprite.fromTexture("objects/FactoryHelper/pressurePlate/plate_case0", entity)
    case:setJustification(0.0, 0.0)
    case:addPosition(-2, -2)

    return {
        case,
        button
    }
end

function pressurePlate.selection(room, entity)
    return utils.rectangle(entity.x, entity.y + 4, 16, 12)
end

return pressurePlate
