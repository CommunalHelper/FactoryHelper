local boomBox = {}

boomBox.name = "FactoryHelper/BoomBox"

boomBox.fieldInformation = {
    initialDelay = {
        minimumValue = 0.0
    }
}

boomBox.placements = {
    name = "active",
    data = {
        activationId = "",
        initialDelay = 0.0,
        startActive = true,
        spriteDir = ""
    }
}

function boomBox.texture(room, entity)
    local spriteDir = (entity.spriteDir or "") == "" and "objects/FactoryHelper/boomBox" or entity.spriteDir
    return spriteDir .. (entity.startActive and "/active00" or "/idle00")
end

boomBox.justification = {0.0, 0.0}

return boomBox
