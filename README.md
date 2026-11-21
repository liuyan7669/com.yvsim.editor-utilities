# Editor Utilities

面向 Unity 项目的纯编辑器工具包，提供资源检查、批处理、源码整理和运行时调试辅助功能。所有代码仅在 Unity Editor 中编译，不会进入 Player 构建。

## 功能

### 组件剪贴板

- 通过复制、剪切和粘贴三个独立菜单传输物体全部组件
- `Transform` 或 `RectTransform` 数值会在复制、剪切时记录，并在粘贴时一起覆盖目标
- 目标存在同类型、同顺序组件时覆盖，不足时自动添加
- 保留外部资源引用，并将源物体内部组件引用映射到目标对应组件
- 剪切时更新已加载场景中指向源组件的外部序列化引用
- 粘贴支持整批 Undo，失败时自动整体回滚
- 同时提供顶部 Yvsim 菜单和 Hierarchy 物体右键 Yvsim 子菜单

顶部菜单入口：`Yvsim > 物体 > 组件 > 复制全部组件 / 剪切全部组件 / 粘贴全部组件`

物体右键入口：`Yvsim > 物体 > 组件 > 复制全部组件 / 剪切全部组件 / 粘贴全部组件`

### 项目诊断与批处理

- `引用查找`：查看目标资源的依赖，并反向扫描引用目标的 Assets/Packages 资源
- `丢失引用扫描`：检查已加载场景、Prefab 和 ScriptableObject 的 Missing Script 与 Missing 引用
- `场景体检`：检查 Missing Script、丢失引用、EventSystem、AudioListener 和 MainCamera
- `Prefab 覆盖管理`：汇总选中 Prefab 实例的 Override，并批量 Apply 或 Revert
- `选择历史与收藏`：会话内前进/后退选择，并将常用对象收藏到 ProjectSettings
- `查看与复制对象信息`：复制资源路径、GUID、Hierarchy 路径、类型和程序集
- `材质批量替换`：替换选中层级或指定目录 Prefab 的 Renderer 材质槽

以上入口按用途分布在 `Yvsim > 资源`、`场景`、`项目` 等分类中。会写入资源的操作均提供确认；场景批处理支持 Undo，Prefab 资源修改会直接保存。

### 复制路径

Hierarchy 与 Project 右键菜单均提供 `Yvsim > 复制路径`：

- Hierarchy：复制右键物体从场景根节点开始的完整层级路径，例如 `/Canvas/Main/Button`。场景物体没有独立磁盘文件，该路径用于定位层级。
- Project：复制所选文件或文件夹的磁盘绝对路径，文件保留扩展名；多选时每项一行。未选中资源时复制当前浏览目录，包资源解析到实际安装目录。
- 结果直接写入系统剪贴板。

### 模型文件导出

- 扫描项目 `Assets` 目录中的模型资源
- 按名称和路径筛选模型
- 将选中的模型及其 `.meta` 文件复制到指定目录
- 导出前检查重名文件和目标覆盖风险

菜单入口：`Yvsim > 资源 > 模型文件导出`

### 运行时类调试器

- 在 Play Mode 中查找当前运行时对象
- 查看对象字段、属性和嵌套值
- 调用可用方法并填写参数
- 支持使用 JSON 输入复杂参数

菜单入口：`Yvsim > 调试 > 运行时对象调试`

### 源码提取工具

- 递归扫描指定目录中的源码文件
- 合并为一个 UTF-8 无 BOM 文本文件
- 按原目录结构复制源码并生成文件清单
- 可选择包含 Unity 当前注册的 Packages 源码
- 不修改或删除源文件

菜单入口：`Yvsim > 项目 > 源码导出`

### Text 字体检查

- 检查当前已加载场景和项目 Prefab
- 同时支持 `UnityEngine.UI.Text` 和 `TMP_Text`
- 查找字体缺失或字体不一致的文本组件
- 支持批量替换字体并记录场景 Undo

菜单入口：`Yvsim > UI > 字体检查与替换`

### 贴图压缩

- 扫描并筛选项目贴图
- 显示尺寸、类型、压缩和内存占用信息
- 批量修改选中贴图的 Max Size
- 贴图资源变化后自动刷新已打开的窗口

菜单入口：`Yvsim > 资源 > 纹理压缩`

## 安装

当前源码版本为 `1.0.0`。准备本包源码，使用本地 UPM 安装：

1. 在 `Window > Package Manager > + > Add package from disk...` 选择本包根目录的 `package.json`。
2. 等待 Unity 解析清单中声明的 Unity 依赖并完成编译。

也可以将完整包目录放入项目的 `Packages/com.yvsim.editor-utilities`，作为 Embedded Package 使用。

本包提供本地源码、Git 和 OpenUPM 安装方式；固定版本使用 `1.0.0`，具体步骤见下文。

## 使用方法

所有工具统一位于 Unity 顶部菜单：

```text
Yvsim > 资源 / 场景 / 项目 / UI / 调试 / 物体
```

### 复制、剪切和粘贴全部组件

1. 在 Hierarchy 中选中源物体。
2. 执行 `Yvsim > 物体 > 组件 > 复制全部组件` 或 `Yvsim > 物体 > 组件 > 剪切全部组件`；也可以使用物体右键菜单中的 `Yvsim > 物体 > 组件 > 复制全部组件 / 剪切全部组件`。
3. 选中一个或多个目标物体。
4. 执行 `Yvsim > 物体 > 组件 > 粘贴全部组件` 或物体右键菜单中的 `Yvsim > 物体 > 组件 > 粘贴全部组件`。
5. 检查 Console 中的覆盖、添加和引用重映射数量。

当前选中多少个目标物体，就会粘贴多少份；全部目标合并为一次 Undo。`Transform` 和 `RectTransform` 不会从源物体删除，但其位置、旋转、缩放以及 RectTransform 锚点布局数据会随剪贴板一起粘贴。为避免布局数据丢失，源和全部目标必须使用相同的 Transform 类型。同类型其他组件按出现顺序对应，目标上的多余组件不会删除。Prefab 资源需要先进入 Prefab Mode 再操作。

剪切到多个目标时，源组件会在全部目标粘贴成功后统一删除；已加载场景中原本指向源组件的外部引用，由当前活动目标物体（批量列表中的第一个目标）承接。每个目标内部的组件引用仍分别映射到该目标自身。

### 导出模型文件

1. 打开 `Yvsim > 资源 > 模型文件导出`。
2. 刷新模型列表，通过名称或路径筛选资源。
3. 勾选需要导出的模型并设置项目外的输出目录。
4. 根据需要选择是否同时导出 `.meta` 文件，然后执行导出。

工具会为每次导出创建独立目录，不会覆盖已有导出结果。为避免导出的模型被 Unity 再次导入，输出目录不能位于当前项目的 `Assets` 目录内。

### 调试运行时类

1. 进入 Play Mode。
2. 打开 `Yvsim > 调试 > 运行时对象调试`。
3. 刷新并搜索需要检查的运行时对象。
4. 打开对象详情查看字段、属性和嵌套值。
5. 需要调用方法时填写参数并执行；复杂参数可以使用 JSON 输入。

运行时方法调用可能直接改变当前运行状态，应优先在测试场景中使用。

### 提取项目源码

1. 打开 `Yvsim > 项目 > 源码导出`。
2. 选择源码目录和项目外的输出目录。
3. 配置扩展名、排除目录以及是否包含 `.meta` 和 Packages 源码。
4. 选择“合并为单个文本”或“按原目录结构复制”。
5. 确认扫描预览后执行提取。

默认扫描项目的 `Assets` 目录，默认输出到项目根目录下的 `SourceCodeExports`。工具只读取源文件，不会修改或删除源文件。

### 检查和替换 Text 字体

1. 打开 `Yvsim > UI > 字体检查与替换`。
2. 刷新当前已加载场景和项目 Prefab 中的文本组件。
3. 使用类型、字体和关键字条件筛选结果。
4. 为需要处理的 `Text` 或 `TMP_Text` 指定新字体。
5. 单独应用或批量替换选中的字体。

场景修改会记录 Undo 并标记为未保存，不会自动保存场景；Prefab 修改会直接保存对应 Prefab 资源。

### 检查和修改贴图压缩

1. 打开 `Yvsim > 资源 > 纹理压缩`。
2. 刷新并筛选项目中的贴图资源。
3. 查看贴图尺寸、类型、压缩方式和内存占用。
4. 选择目标 Max Size 并应用到选中的贴图。

应用 Max Size 会修改 `TextureImporter` 并重新导入贴图，批量处理前应确认筛选范围和选中资源。

## 可导入示例

在 Package Manager 导入 **Tool Playground**，先保存当前场景（包括 Untitled），再执行 `Yvsim > 示例 > Editor Utilities > 创建并打开工具练习场景`。示例在新的 `Assets/YvsimSamples/EditorUtilitiesPlayground*` 目录生成 Prefab、两种材质、棋盘格贴图和练习场景，重复生成不会覆盖已有素材。

按 [示例说明](Samples~/Tool%20Playground/README.md) 使用现有工具练习组件复制、Prefab 覆盖、引用查找、材质替换、路径复制和纹理尺寸调整。示例只有 Editor 菜单生成器，不新增 Inspector 或窗口，也不依赖 Groups；生成器支持 Built-in/URP，使用 Unity 标准的 Physics 与 Image Conversion 模块。

## 主要工具

| 类型 | 说明 |
| --- | --- |
| `ComponentClipboardMenu` | 通过顶部菜单和 Hierarchy 物体右键菜单复制、剪切、粘贴全部组件 |
| `ReferenceFinderWindow` | 查看资源依赖并反向查找引用资源 |
| `MissingReferenceScannerWindow` | 扫描场景、Prefab 和 ScriptableObject 的丢失引用 |
| `SceneValidationWindow` | 检查已加载场景的常见配置问题 |
| `PrefabOverrideManagerWindow` | 汇总并批量 Apply/Revert Prefab Override |
| `SelectionHistoryWindow` | 提供选择历史、前进后退与持久收藏 |
| `AssetInfoWindow` | 复制资源和 Hierarchy 对象的定位信息 |
| `MaterialReplacerWindow` | 批量替换场景层级或 Prefab 中的材质 |
| `ModelFileExporterWindow` | 扫描、筛选并导出模型源文件 |
| `RuntimeClassDebuggerWindow` | 查找和检查 Play Mode 中的运行时对象 |
| `SourceCodeExtractorWindow` | 合并或按目录结构复制项目源码 |
| `TextFontScannerWindow` | 检查并替换场景和 Prefab 中的 Text/TMP 字体 |
| `TextureCompressionWindow` | 检查贴图信息并批量修改 Max Size |

## 注意事项

- 本包全部代码只在 Unity Editor 中编译，不会进入 Player 构建。
- 组件剪切会在粘贴成功后删除源物体上的非 Transform 组件；Transform 始终保留，整次粘贴支持 Undo。
- 模型导出和源码提取默认不会覆盖已有输出结果。
- Text 字体批量替换会修改场景或 Prefab，执行前应确认筛选范围。
- 贴图 Max Size 修改会触发资源重新导入，大批量处理可能需要一定时间。
- 运行时类调试器使用反射读取对象和调用方法，仅建议用于开发与测试。

## 环境要求

- Unity 2021.3 或更高版本
- Newtonsoft Json `3.2.2`
- TextMeshPro `3.0.6`
- Unity UI `1.0.0`

## 许可证

本包使用 MIT License，详情参见仓库中的许可证文件。

## 依赖说明

本包不依赖 Inspector Groups 或其他 Yvsim 包。

## 从 GitHub 安装

仓库：[com.yvsim.editor-utilities](https://github.com/liuyan7669/com.yvsim.editor-utilities)。仓库名与 UPM 包名一致。

在 Unity Package Manager 选择 Add package from git URL，输入：

```text
https://github.com/liuyan7669/com.yvsim.editor-utilities.git#main
```

此地址跟随 main 分支。需要固定 1.0.0 源码时，将 `#main` 改为 `#v1.0.0`；Unity 官方依赖仍按 package.json 解析。

## 从 OpenUPM 安装

在 Unity 的 Project Settings > Package Manager 中添加 Scoped Registry：

- Name：OpenUPM
- URL：`https://package.openupm.com`
- Scope：`com.yvsim`

然后在 Package Manager 中通过 Add package by name 输入 `com.yvsim.editor-utilities`，版本填 `1.0.0`。也可从 My Registries 中选择本包。声明的 Yvsim 依赖由 OpenUPM 解析，Unity 官方依赖仍由 Unity 注册表提供。

如果已安装 OpenUPM CLI，可在工程根目录执行：

```sh
openupm add com.yvsim.editor-utilities@1.0.0
```

[OpenUPM 包页面](https://openupm.com/packages/com.yvsim.editor-utilities/)
