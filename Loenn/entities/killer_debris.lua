local utils = require("utils")

local killerDebris = {}

local debrisColors = {
    "Bronze",
    "Silver"
}

killerDebris.name = "FactoryHelper/KillerDebris"

killerDebris.fieldInformation = {
    color = {
        editable = false,
        options = debrisColors
    }
}

killerDebris.placements = {}
for _, color in ipairs(debrisColors) do
    table.insert(killerDebris.placements, {
        name = string.lower(color),
        data = {
            color = color,
            attachToSolid = false,
        }
    })
end

function killerDebris.texture(room, entity)
    local color = entity.color or "Bronze"
    return string.format("danger/FactoryHelper/debris/fg_%s1", string.lower(color))
end

function killerDebris.selection(room, entity)
    return utils.rectangle(entity.x - 9, entity.y - 9, 18, 18)
end

return killerDebris
