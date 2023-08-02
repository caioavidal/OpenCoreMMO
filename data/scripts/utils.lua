function typeNameOf(object)
    if object == nil then
        return nil
    end
    
    return object:GetType().Name
end


function addToSet(set, key)
    set[key] = true
end

function removeFromSet(set, key)
    set[key] = nil
end

function setContains(set, key)
    return set[key] ~= nil
end

function createCondition(params)
    local condition = Condition(params.type, params.duration)

    condition.StartAction = params.onStart
    condition.EndAction = params.onEnd

    return condition
end