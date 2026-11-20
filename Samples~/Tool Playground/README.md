# Tool Playground

为本包现有工具准备可重复生成的练习素材。没有新增工具窗口、自定义 Inspector 或运行时业务脚本，也不依赖 Inspector Groups 或其他 Yvsim 包。

## 创建练习场景

1. 在 Package Manager 的 Editor Utilities 页面导入 **Tool Playground**，等待编译。
2. 先保存当前场景（包括未修改的 Untitled 场景），再执行 `Yvsim > 示例 > Editor Utilities > 创建并打开工具练习场景`。如当前场景未保存，先按 Unity 提示保存或取消。
3. 在生成的场景和 Project 素材目录中，按下面的操作使用本包工具，无需进入 Play Mode。

菜单使用 Unity API 创建新的 `Assets/YvsimSamples/EditorUtilitiesPlayground*` 目录，包含：

- `ToolPlayground.unity`：两个 Prefab 实例、一个空的粘贴目标、Camera 与 Light。
- `SampleCube.prefab`：包含 MeshFilter、MeshRenderer、BoxCollider、Rigidbody 的基础立方体。
- `Orange.mat` 和 `Blue.mat`：供引用查找和材质替换使用的两种材质。
- `Checker.png`：128×128 棋盘格贴图，由材质引用，可供纹理工具调整尺寸。

每次生成都使用唯一目录，保留先前的练习结果，不修改其他资源或 Build Settings。素材生成器适用于 Built-in Standard 或 URP Lit；不安装 URP，也不更改渲染管线。其他管线可自行替换生成器的 Shader 选择。

## 练习与预期结果

| 现有工具 | 操作 | 观察结果 |
| --- | --- | --- |
| 组件剪贴板 | 选中 `Source - Copy Components or Inspect Overrides`，执行 `Yvsim > 物体 > 组件 > 复制全部组件`；再选中 `Paste Target - Initially Empty` 执行粘贴 | 空目标获得渲染、碰撞和刚体组件，并保留材质引用。Transform 也被复制，目标会与源重合；粘贴后可手动移动。一次 Undo 可撤销整批粘贴 |
| Prefab 覆盖管理 | 选中 Source，打开 `Yvsim > 场景 > Prefab 覆盖管理` | 能看到示例预先修改的 BoxCollider `Is Trigger`；可在练习 Prefab 上 Apply 或 Revert。Revert 后该值恢复为关闭 |
| 引用查找 | 选中 `Orange.mat`，打开 `Yvsim > 资源 > 引用查找` 并执行扫描 | 可检查材质依赖的 `Checker.png`，并查找引用该材质的示例 Prefab/场景 |
| 材质批量替换 | 选中 `Tool Playground` 根对象，打开 `Yvsim > 资源 > 材质批量替换`，旧材质选择 Orange、新材质选择 Blue，使用所选层级范围并包含子物体，先扫描预览再执行 | 两个立方体材质切换为 Blue；可 Undo。Prefab 目录模式会保存资源，练习时仅指定本次生成的目录 |
| 丢失引用扫描 | 打开 `Yvsim > 场景 > 丢失引用扫描`，仅勾选“扫描当前已加载场景”，关闭 Prefab 与 ScriptableObject 扫描 | 原始练习数据没有刻意制造 Missing Script / Missing 引用；可以检查正常资源基线 |
| 查看与复制对象信息 | 分别选择 Source、`SampleCube.prefab`，打开 `Yvsim > 项目 > 查看与复制对象信息` | 场景物体可查看层级路径；资源可查看资源路径、GUID 和类型信息 |
| 复制路径 | 在 Hierarchy 右键 Source、在 Project 右键 Checker，分别使用 `Yvsim > 复制路径` | 分别得到场景层级路径、包含扩展名的磁盘绝对路径 |
| 纹理压缩 | 选择 Checker，打开 `Yvsim > 资源 > 纹理压缩`，在搜索框输入本次生成目录路径，先清空选择，再勾选 Checker 并将 Max Size 改为 64 | TextureImporter 的 Max Size 改变，贴图重新导入；本操作会保存导入设置 |

资源批处理只选本次生成的目录；工具中的 Apply、材质资源替换、贴图导入设置属于实际修改。场景操作按现有工具支持 Undo，资源操作按其确认提示执行。此示例没有伪造丢失 GUID，也不会自动执行批处理。

示例生成器使用 Unity 标准的 `com.unity.modules.physics` 和 `com.unity.modules.imageconversion` 模块（均为 `1.0.0`）。常规 Unity 项目默认包含它们；若项目主动移除了这些内置模块，导入示例前请在 Package Manager 中恢复。工具包本体不新增这些示例依赖。

## 代码与 CLI

示例程序集 `Yvsim.EditorUtilities.Samples.Editor` 只在 Editor 编译。菜单仅生成素材，所有练习仍通过本包现有工具完成，因此不需要额外程序集引用。

CLI 调用前也必须将当前未命名场景保存到明确路径，或关闭该场景；公开生成方法遇到未保存命名的场景时会在创建任何示例文件前抛出可操作的异常，不弹出窗口或自动保存用户场景。

CLI 可调用 `Yvsim.EditorUtilities.Samples.Editor.EditorUtilitiesSampleBuilder.CreateSampleScene()`。它返回生成的场景资源路径，生成后恢复原有场景集合；顶部菜单会进一步打开该场景。
