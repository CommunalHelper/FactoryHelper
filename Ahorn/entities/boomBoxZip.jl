module FactoryHelperBoomBoxZip
using ..Ahorn, Maple

@mapdef Entity "FactoryHelper/BoomBoxZip" BoomBoxZip(x::Integer, y::Integer, activationId::String="", initialDelay::Number=0.0, startActive::Bool=false) 

const placements = Ahorn.PlacementDict(
    "Boom Box Zip (Factory Helper)" => Ahorn.EntityPlacement(
        BoomBoxZip,
        "point",
        Dict{String, Any}(
			"startActive" => true
		),
        function(entity)
            entity.data["nodes"] = [(Int(entity.data["x"]) + 32, Int(entity.data["y"]))]
        end
    ),
)
activeSprite = "objects/FactoryHelper/boomBox/active00"

Ahorn.nodeLimits(entity::BoomBoxZip) = 1,1

ropeColor = (77, 60, 34) ./ 255
function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::BoomBoxZip, room::Maple.Room)
    x, y = Ahorn.position(entity)

    Ahorn.drawSprite(ctx, activeSprite, x + 12, y + 12)

    nx, ny = Int.(entity.data["nodes"][1])

    cx, cy = x + 12, y + 12
    cnx,cny = nx + 12, ny + 12
    length = sqrt((x - nx)^2 + (y - ny)^2)
    theta = atan(cny - cy, cnx - cx)
    Ahorn.Cairo.save(ctx)

    Ahorn.translate(ctx, cx, cy)
    Ahorn.rotate(ctx, theta)

    Ahorn.setSourceColor(ctx, ropeColor)
    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    # Offset for rounding errors
    Ahorn.move_to(ctx, 0, 4 + (theta <= 0))
    Ahorn.line_to(ctx, length, 4 + (theta <= 0))

    Ahorn.move_to(ctx, 0, -4 - (theta > 0))
    Ahorn.line_to(ctx, length, -4 - (theta > 0))

    Ahorn.stroke(ctx)

    Ahorn.Cairo.restore(ctx)
    
    Ahorn.drawSprite(ctx, "objects/zipmover/cog", cnx, cny)

end

function Ahorn.selection(entity::BoomBoxZip)
    x, y = Ahorn.position(entity)
    nx, ny = Int.(entity.data["nodes"][1])

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))

    return [Ahorn.Rectangle(x, y, 24, 24), Ahorn.Rectangle(nx + floor(Int, width / 2) - 5, ny + floor(Int, height / 2) - 5, 10, 10)]
end


end