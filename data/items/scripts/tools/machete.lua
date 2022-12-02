machete = {}

function machete.register()
	register(2420, machete.use)
end

function machete.use(machete, usedBy, onItem)
	newItem = itemTransformer:Transform(map[onItem.Location],2739,2737)
	newItem.Decayable:StartDecay()
	
	return true
end

machete.register()