---@class System.Type : System.Reflection.MemberInfo
---@field IsSerializable bool
---@field ContainsGenericParameters bool
---@field IsVisible bool
---@field MemberType System.Reflection.MemberTypes
---@field Namespace string
---@field AssemblyQualifiedName string
---@field FullName string
---@field Assembly System.Reflection.Assembly
---@field Module System.Reflection.Module
---@field IsNested bool
---@field DeclaringType System.Type
---@field DeclaringMethod System.Reflection.MethodBase
---@field ReflectedType System.Type
---@field UnderlyingSystemType System.Type
---@field IsTypeDefinition bool
---@field IsArray bool
---@field IsByRef bool
---@field IsPointer bool
---@field IsConstructedGenericType bool
---@field IsGenericParameter bool
---@field IsGenericTypeParameter bool
---@field IsGenericMethodParameter bool
---@field IsGenericType bool
---@field IsGenericTypeDefinition bool
---@field IsSZArray bool
---@field IsVariableBoundArray bool
---@field IsByRefLike bool
---@field HasElementType bool
---@field GenericTypeArguments table
---@field GenericParameterPosition int
---@field GenericParameterAttributes System.Reflection.GenericParameterAttributes
---@field Attributes System.Reflection.TypeAttributes
---@field IsAbstract bool
---@field IsImport bool
---@field IsSealed bool
---@field IsSpecialName bool
---@field IsClass bool
---@field IsNestedAssembly bool
---@field IsNestedFamANDAssem bool
---@field IsNestedFamily bool
---@field IsNestedFamORAssem bool
---@field IsNestedPrivate bool
---@field IsNestedPublic bool
---@field IsNotPublic bool
---@field IsPublic bool
---@field IsAutoLayout bool
---@field IsExplicitLayout bool
---@field IsLayoutSequential bool
---@field IsAnsiClass bool
---@field IsAutoClass bool
---@field IsUnicodeClass bool
---@field IsCOMObject bool
---@field IsContextful bool
---@field IsCollectible bool
---@field IsEnum bool
---@field IsMarshalByRef bool
---@field IsPrimitive bool
---@field IsValueType bool
---@field IsSignatureType bool
---@field IsSecurityCritical bool
---@field IsSecuritySafeCritical bool
---@field IsSecurityTransparent bool
---@field StructLayoutAttribute System.Runtime.InteropServices.StructLayoutAttribute
---@field TypeInitializer System.Reflection.ConstructorInfo
---@field TypeHandle System.RuntimeTypeHandle
---@field GUID System.Guid
---@field BaseType System.Type
---@field DefaultBinder System.Reflection.Binder
---@field IsInterface bool
---@field Delimiter char
---@field EmptyTypes table
---@field Missing object
---@field FilterAttribute System.Reflection.MemberFilter
---@field FilterName System.Reflection.MemberFilter
---@field FilterNameIgnoreCase System.Reflection.MemberFilter
local m = {}
---@param value object
---@return bool
function m:IsEnumDefined(value) end
---@param value object
---@return string
function m:GetEnumName(value) end
---@return table
function m:GetEnumNames() end
---@param filter System.Reflection.TypeFilter
---@param filterCriteria object
---@return table
function m:FindInterfaces(filter, filterCriteria) end
---@param memberType System.Reflection.MemberTypes
---@param bindingAttr System.Reflection.BindingFlags
---@param filter System.Reflection.MemberFilter
---@param filterCriteria object
---@return table
function m:FindMembers(memberType, bindingAttr, filter, filterCriteria) end
---@param c System.Type
---@return bool
function m:IsSubclassOf(c) end
---@param c System.Type
---@return bool
function m:IsAssignableFrom(c) end
---@overload fun(typeName:string, throwOnError:bool, ignoreCase:bool):System.Type
---@overload fun(typeName:string, throwOnError:bool):System.Type
---@overload fun(typeName:string):System.Type
---@overload fun(typeName:string, assemblyResolver:System.Func, typeResolver:System.Func):System.Type
---@overload fun(typeName:string, assemblyResolver:System.Func, typeResolver:System.Func, throwOnError:bool):System.Type
---@overload fun(typeName:string, assemblyResolver:System.Func, typeResolver:System.Func, throwOnError:bool, ignoreCase:bool):System.Type
---@return System.Type
function m:GetType() end
---@return System.Type
function m:GetElementType() end
---@return int
function m:GetArrayRank() end
---@return System.Type
function m:GetGenericTypeDefinition() end
---@return table
function m:GetGenericArguments() end
---@return table
function m:GetGenericParameterConstraints() end
---@overload fun(bindingAttr:System.Reflection.BindingFlags, binder:System.Reflection.Binder, types:table, modifiers:table):System.Reflection.ConstructorInfo
---@overload fun(bindingAttr:System.Reflection.BindingFlags, binder:System.Reflection.Binder, callConvention:System.Reflection.CallingConventions, types:table, modifiers:table):System.Reflection.ConstructorInfo
---@param types table
---@return System.Reflection.ConstructorInfo
function m:GetConstructor(types) end
---@overload fun(bindingAttr:System.Reflection.BindingFlags):table
---@return table
function m:GetConstructors() end
---@overload fun(name:string, bindingAttr:System.Reflection.BindingFlags):System.Reflection.EventInfo
---@param name string
---@return System.Reflection.EventInfo
function m:GetEvent(name) end
---@overload fun(bindingAttr:System.Reflection.BindingFlags):table
---@return table
function m:GetEvents() end
---@overload fun(name:string, bindingAttr:System.Reflection.BindingFlags):System.Reflection.FieldInfo
---@param name string
---@return System.Reflection.FieldInfo
function m:GetField(name) end
---@overload fun(bindingAttr:System.Reflection.BindingFlags):table
---@return table
function m:GetFields() end
---@overload fun(name:string, bindingAttr:System.Reflection.BindingFlags):table
---@overload fun(name:string, type:System.Reflection.MemberTypes, bindingAttr:System.Reflection.BindingFlags):table
---@param name string
---@return table
function m:GetMember(name) end
---@overload fun(bindingAttr:System.Reflection.BindingFlags):table
---@return table
function m:GetMembers() end
---@overload fun(name:string, bindingAttr:System.Reflection.BindingFlags):System.Reflection.MethodInfo
---@overload fun(name:string, types:table):System.Reflection.MethodInfo
---@overload fun(name:string, types:table, modifiers:table):System.Reflection.MethodInfo
---@overload fun(name:string, bindingAttr:System.Reflection.BindingFlags, binder:System.Reflection.Binder, types:table, modifiers:table):System.Reflection.MethodInfo
---@overload fun(name:string, bindingAttr:System.Reflection.BindingFlags, binder:System.Reflection.Binder, callConvention:System.Reflection.CallingConventions, types:table, modifiers:table):System.Reflection.MethodInfo
---@overload fun(name:string, genericParameterCount:int, types:table):System.Reflection.MethodInfo
---@overload fun(name:string, genericParameterCount:int, types:table, modifiers:table):System.Reflection.MethodInfo
---@overload fun(name:string, genericParameterCount:int, bindingAttr:System.Reflection.BindingFlags, binder:System.Reflection.Binder, types:table, modifiers:table):System.Reflection.MethodInfo
---@overload fun(name:string, genericParameterCount:int, bindingAttr:System.Reflection.BindingFlags, binder:System.Reflection.Binder, callConvention:System.Reflection.CallingConventions, types:table, modifiers:table):System.Reflection.MethodInfo
---@param name string
---@return System.Reflection.MethodInfo
function m:GetMethod(name) end
---@overload fun(bindingAttr:System.Reflection.BindingFlags):table
---@return table
function m:GetMethods() end
---@overload fun(name:string, bindingAttr:System.Reflection.BindingFlags):System.Type
---@param name string
---@return System.Type
function m:GetNestedType(name) end
---@overload fun(bindingAttr:System.Reflection.BindingFlags):table
---@return table
function m:GetNestedTypes() end
---@overload fun(name:string, bindingAttr:System.Reflection.BindingFlags):System.Reflection.PropertyInfo
---@overload fun(name:string, returnType:System.Type):System.Reflection.PropertyInfo
---@overload fun(name:string, types:table):System.Reflection.PropertyInfo
---@overload fun(name:string, returnType:System.Type, types:table):System.Reflection.PropertyInfo
---@overload fun(name:string, returnType:System.Type, types:table, modifiers:table):System.Reflection.PropertyInfo
---@overload fun(name:string, bindingAttr:System.Reflection.BindingFlags, binder:System.Reflection.Binder, returnType:System.Type, types:table, modifiers:table):System.Reflection.PropertyInfo
---@param name string
---@return System.Reflection.PropertyInfo
function m:GetProperty(name) end
---@overload fun(bindingAttr:System.Reflection.BindingFlags):table
---@return table
function m:GetProperties() end
---@return table
function m:GetDefaultMembers() end
---@param o object
---@return System.RuntimeTypeHandle
function m.GetTypeHandle(o) end
---@param args table
---@return table
function m.GetTypeArray(args) end
---@param type System.Type
---@return System.TypeCode
function m.GetTypeCode(type) end
---@overload fun(clsid:System.Guid, throwOnError:bool):System.Type
---@overload fun(clsid:System.Guid, server:string):System.Type
---@overload fun(clsid:System.Guid, server:string, throwOnError:bool):System.Type
---@param clsid System.Guid
---@return System.Type
function m.GetTypeFromCLSID(clsid) end
---@overload fun(progID:string, throwOnError:bool):System.Type
---@overload fun(progID:string, server:string):System.Type
---@overload fun(progID:string, server:string, throwOnError:bool):System.Type
---@param progID string
---@return System.Type
function m.GetTypeFromProgID(progID) end
---@overload fun(name:string, invokeAttr:System.Reflection.BindingFlags, binder:System.Reflection.Binder, target:object, args:table, culture:System.Globalization.CultureInfo):object
---@overload fun(name:string, invokeAttr:System.Reflection.BindingFlags, binder:System.Reflection.Binder, target:object, args:table, modifiers:table, culture:System.Globalization.CultureInfo, namedParameters:table):object
---@param name string
---@param invokeAttr System.Reflection.BindingFlags
---@param binder System.Reflection.Binder
---@param target object
---@param args table
---@return object
function m:InvokeMember(name, invokeAttr, binder, target, args) end
---@overload fun(name:string, ignoreCase:bool):System.Type
---@param name string
---@return System.Type
function m:GetInterface(name) end
---@return table
function m:GetInterfaces() end
---@param interfaceType System.Type
---@return System.Reflection.InterfaceMapping
function m:GetInterfaceMap(interfaceType) end
---@param o object
---@return bool
function m:IsInstanceOfType(o) end
---@param other System.Type
---@return bool
function m:IsEquivalentTo(other) end
---@return System.Type
function m:GetEnumUnderlyingType() end
---@return table
function m:GetEnumValues() end
---@overload fun(rank:int):System.Type
---@return System.Type
function m:MakeArrayType() end
---@return System.Type
function m:MakeByRefType() end
---@param typeArguments table
---@return System.Type
function m:MakeGenericType(typeArguments) end
---@return System.Type
function m:MakePointerType() end
---@param genericTypeDefinition System.Type
---@param typeArguments table
---@return System.Type
function m.MakeGenericSignatureType(genericTypeDefinition, typeArguments) end
---@param position int
---@return System.Type
function m.MakeGenericMethodParameter(position) end
---@return string
function m:ToString() end
---@overload fun(o:System.Type):bool
---@param o object
---@return bool
function m:Equals(o) end
---@return int
function m:GetHashCode() end
---@param handle System.RuntimeTypeHandle
---@return System.Type
function m.GetTypeFromHandle(handle) end
---@param left System.Type
---@param right System.Type
---@return bool
function m.op_Equality(left, right) end
---@param left System.Type
---@param right System.Type
---@return bool
function m.op_Inequality(left, right) end
---@param typeName string
---@param throwIfNotFound bool
---@param ignoreCase bool
---@return System.Type
function m.ReflectionOnlyGetType(typeName, throwIfNotFound, ignoreCase) end
System = {}
System.Type = m
return m