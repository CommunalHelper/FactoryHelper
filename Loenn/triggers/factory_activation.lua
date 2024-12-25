local factoryActivation = {}

local modes = {
    "Activate",
    "Deactivate"
}

factoryActivation.name = "FactoryHelper/FactoryActivationTrigger"
factoryActivation.placements = {
    name = "factory_activation",
    data = {
        mode = "Activate",
        activationIds = "",
        ownActivationId = "",
        lockState = true,
        persistent = false
    }
}
factoryActivation.fieldInformation = {
    mode = {
        editable = false,
        options = modes
    }
}

return factoryActivation
