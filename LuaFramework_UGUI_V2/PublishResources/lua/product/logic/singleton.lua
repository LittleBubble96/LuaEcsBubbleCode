--@class Singleton:On
Singleton = class("Singleton")
function Singleton:GetInstance()
    if not self._instance then
        self._instance = self.new()
    end    
    return self._instance
end
return Singleton

