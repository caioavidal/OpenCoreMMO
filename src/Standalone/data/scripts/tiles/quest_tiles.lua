quest_tiles = {}

function quest_tiles.cantEnter(creature) return false end

function quest_tiles.canMove(player, item) 
  sendOperationFail(player, "You cannot move this item.")
  return false 
end

tile_helper.addEnterRule(Location(32149,32105,11), quest_tiles.cantEnter)
--register("canMove", quest_tiles.canMove, 1740, Location(32149,32105,11))
