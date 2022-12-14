module FactoryHelperKillerDebris

using ..Ahorn, Maple

@mapdef Entity "FactoryHelper/KillerDebris" KillerDebris(x::Integer, y::Integer, color::String="Bronze", attachToSolid::Bool=false)

const placements = Ahorn.PlacementDict(
)

debrisColors = String["Bronze", "Silver"]

Ahorn.editingOptions(entity::KillerDebris) = Dict{String, Any}(
    "color" => debrisColors
)

for color in debrisColors
    key = "Killer Debris ($(uppercasefirst(color))) (FactoryHelper)"
    placements[key] = Ahorn.EntityPlacement(
        KillerDebris,
        "point",
        Dict{String, Any}(
            "color" => color
        )
    )
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::KillerDebris, room::Maple.Room)
    
    color = lowercase(get(entity.data, "color", "bronze"))
	sprite = "danger/FactoryHelper/debris/fg_$(color)1"
    
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

function Ahorn.selection(entity::KillerDebris)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 8, y - 8, 16, 16)
end

end