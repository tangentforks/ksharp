using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using K3CSharp;

namespace KMCPServer
{
    public class KMCPServer
    {
        private readonly KInterpreterWrapper wrapper;

        public KMCPServer()
        {
            this.wrapper = new KInterpreterWrapper();
        }

        public void Start()
        {
            Console.WriteLine("K MCP Server starting...");
            
            using var reader = new StreamReader(Console.OpenStandardInput());
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;

                JsonRpcRequest request = null;
                try
                {
                    request = JsonSerializer.Deserialize<JsonRpcRequest>(line);
                    var response = ProcessRequest(request);
                    
                    Console.WriteLine("\x01\x02" + JsonSerializer.Serialize(response) + "\x03");
                }
                catch (Exception ex)
                {
                    var errorResponse = new JsonRpcResponse
                    {
                        id = request?.id,
                        error = new JsonRpcError { code = -32603, message = string.Format("Internal error: {0}", ex.Message) }
                    };
                    Console.WriteLine("\x01\x02" + JsonSerializer.Serialize(errorResponse) + "\x03");
                }
            }
        }

        private JsonRpcResponse ProcessRequest(JsonRpcRequest request)
        {
            if (request.method == "initialize")
            {
                var capabilities = new
                {
                    tools = new object[0],
                    resources = new object[0]
                };
                var serverInfo = new
                {
                    name = "k.exe MCP Server",
                    version = "1.0.0"
                };
                
                return new JsonRpcResponse
                {
                    id = request.id,
                    result = new Dictionary<string, object>
                    {
                        ["protocolVersion"] = "2024-11-05",
                        ["capabilities"] = capabilities,
                        ["serverInfo"] = serverInfo
                    }
                };
            }

            if (request.method == "tools/list")
            {
                return new JsonRpcResponse
                {
                    id = request.id,
                    result = new object[0]
                };
            }

            if (request.method == "tools/call")
            {
                return ExecuteTool(request);
            }

            return new JsonRpcResponse
            {
                id = request.id,
                error = new JsonRpcError { code = -32601, message = "Method not found" }
            };
        }

        private JsonRpcResponse ExecuteTool(JsonRpcRequest request)
        {
            try
            {
                var arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(request.@params.ToString());
                var toolName = arguments["name"].ToString();
                var toolArgs = JsonSerializer.Deserialize<string[]>(arguments["arguments"].ToString());

                if (toolName == "execute_k_command")
                {
                    var command = toolArgs.Length > 0 ? toolArgs[0] : "";
                    
                    try
                    {
                        var result = wrapper.ExecuteScript(command);
                        return new JsonRpcResponse 
                        { 
                            id = request.id, 
                            result = new Dictionary<string, object> { ["output"] = result }
                        };
                    }
                    catch (Exception ex)
                    {
                        return new JsonRpcResponse 
                        { 
                            id = request.id, 
                            error = new JsonRpcError { code = -32000, message = string.Format("K execution failed: {0}", ex.Message) } 
                        };
                    }
                }

                if (toolName == "execute_k_script")
                {
                    if (toolArgs.Length == 0)
                    {
                        return new JsonRpcResponse 
                        { 
                            id = request.id, 
                            error = new JsonRpcError { code = -32602, message = "Script content required" } 
                        };
                    }

                    var scriptContent = toolArgs[0];
                    try
                    {
                        var result = wrapper.ExecuteScript(scriptContent);
                        return new JsonRpcResponse 
                        { 
                            id = request.id, 
                            result = new Dictionary<string, object> { ["output"] = result }
                        };
                    }
                    catch (Exception ex)
                    {
                        return new JsonRpcResponse 
                        { 
                            id = request.id, 
                            error = new JsonRpcError { code = -32000, message = string.Format("Script execution failed: {0}", ex.Message) } 
                        };
                    }
                }
                else
                {
                    return new JsonRpcResponse 
                    { 
                        id = request.id, 
                        error = new JsonRpcError { code = -32601, message = "Tool not found" } 
                    };
                }
            }
            catch (Exception ex)
            {
                return new JsonRpcResponse 
                { 
                    id = request.id, 
                    error = new JsonRpcError { code = -32000, message = string.Format("Tool execution failed: {0}", ex.Message) } 
                };
            }
        }
    }

    public class JsonRpcRequest
    {
        public string jsonrpc { get; set; } = "2.0";
        public string method { get; set; }
        public object @params { get; set; }
        public string id { get; set; }
    }

    public class JsonRpcResponse
    {
        public string jsonrpc { get; set; } = "2.0";
        public object result { get; set; }
        public JsonRpcError error { get; set; }
        public string id { get; set; }
    }

    public class JsonRpcError
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}
