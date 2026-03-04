# 问题记录 (FAQ)

## 目录
- [通用](#通用)
- [OpenMCP](#openmcp)

## 通用

### Q: 为什么使用红色方块作为掉血特效？
- **问题描述**: 用户没有提供美术特效资源。
- **原因**: 需要快速实现视觉反馈以验证战斗逻辑。
- **解决方案**: 使用 Unity 原生 Cube + 红色 Material，脚本控制自动销毁。

## OpenMCP

### Q: 为什么会出现 `curl: (7) Failed to connect to localhost port 23456`？
- **问题描述**: 尝试连接 OpenMCP 接口时失败，提示无法连接到服务器。
- **原因**:
    1.  **Unity Editor 未启动或已关闭**：插件仅在 Editor 运行时工作。
    2.  **正在编译脚本 (Domain Reload)**：每次修改 C# 脚本后，Unity 会重新编译。在此期间，OpenMCP 服务会主动停止，直到编译完成并重新加载域。
    3.  **后台挂起 (App Nap)**：MacOS 系统可能会暂停后台运行的 Unity Editor 进程以省电。
    4.  **端口冲突**：23456 端口被其他程序占用。
- **解决方案**:
    1.  **保持 Editor 运行**：确保 Unity Editor 处于打开状态。
    2.  **等待编译完成**：观察 Editor 右下角的小微调图标，等待其消失。
    3.  **强制后台运行**：已添加 `Assets/Editor/OpenMCPDiagnostics.cs` 脚本，强制设置 `Application.runInBackground = true`。
    4.  **手动重启服务**：在 Unity 菜单栏选择 `OpenMCP > Diagnostics > Restart Server`。
    5.  **使用检测脚本**：运行 `./check_mcp.sh` 等待服务就绪。

### Q: 为什么修改代码后 OpenMCP 连接断开且不自动恢复（需要点击 Unity）？
- **问题描述**: 外部修改 C# 脚本后，OpenMCP 连接中断，且 Unity 没有自动触发编译，直到手动点击 Unity 窗口才开始编译并恢复连接。
- **原因**: Unity 在后台运行时，默认的文件系统监控可能存在延迟，或者 "Auto Refresh" 机制未能及时检测到外部文件的变化。这导致 Unity 停留在旧状态，而 OpenMCP 可能因等待编译或状态不一致而无法响应，或者 OpenMCP 实际上还活着但后续操作需要新代码生效。点击 Unity 会强制触发 `AssetDatabase.Refresh()`，从而开始编译（关闭 Server）-> 编译结束（重启 Server）。
- **解决方案**:
    1.  **主动触发刷新**: 在修改脚本后，通过 OpenMCP 的 `/api/v1/asset/refresh` 接口强制 Unity 刷新。这会立即触发编译（并暂时断开连接），但能确保流程自动化继续，无需人工干预。
    2.  **确保 Auto Refresh 开启**: 检查 Unity `Preferences > General > Auto Refresh` 是否勾选。
