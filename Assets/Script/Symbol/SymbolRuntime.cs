using System;
using System.Collections.Generic;
[Serializable]
public class SymbolRuntime
{
    public SymbolSO baseData;        // 配置
    public int intervalCounter = 0;  // 间隔计数
    public SymbolInstance inst;
    public SymbolRuntime(SymbolInstance instant)
    {
        baseData = instant.config;
        inst = instant;
    }
}