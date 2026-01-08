# 情绪碎片

## 项目简介

《情绪碎片》是一款探索情感体验的互动艺术游戏。通过操控漂浮的碎片，玩家将经历"压抑→混乱→重构"的情感旅程，体验从束缚到自由的心理转变过程。

## 游戏玩法

### 核心机制

游戏围绕四个核心情绪维度展开：

- **压抑（Constraint）** - 代表束缚感和限制
- **焦虑（Anxiety）** - 代表混乱和不确定性
- **能动性（Agency）** - 代表控制力和行动能力
- **希望（Hope）** - 代表成长和重建的可能性

### 游戏阶段

#### 1. 秩序阶段（压抑）

- 碎片呈现有序排列，运动规律
- 玩家感受到环境的束缚
- 通过尝试移动和创造来积累希望值
- 触碰秩序核心将触发阶段转换

#### 2. 混乱阶段（焦虑）

- 碎片开始无序运动，环境变得不稳定
- 玩家需要使用脉冲技能来增强控制力
- 避免碎片碰撞以控制焦虑值
- 当焦虑值足够高且具备足够控制力时，进入重构阶段

#### 3. 重构阶段（希望）

- 碎片开始相互吸引，形成新的秩序
- 玩家可以放置碎片，创造新的模式
- 通过创造和重组来积累希望值
- 最终完成心理重建

### 玩家操作

- **移动** - 控制角色在空间中移动
- **创造** - 尝试创造新的碎片或模式
- **脉冲** - 在混乱阶段使用，增强控制力
- **放置** - 在重构阶段放置碎片
- **R键** - 重新开始游戏

## 项目结构

```
游戏/
├── Assets/                          # 游戏资源目录（核心文件，必须保留）
│   ├── Materials/                   # 材质资源
│   ├── Prefabs/                     # 预制体
│   │   └── Fragment.prefab          # 碎片预制体
│   ├── Scenes/                      # 游戏场景
│   │   └── MainScene.unity          # 主场景
│   ├── Scripts/                     # 脚本代码
│   │   ├── AudioManager.cs         # 音频管理器
│   │   ├── EmotionMaterialUpdater.cs  # 情绪材质更新器
│   │   ├── EmotionStateMachine.cs  # 情绪状态机
│   │   ├── EmotionSystemTester.cs  # 情绪系统测试器
│   │   ├── EmotionUIPanel.cs        # 情绪UI面板
│   │   ├── FMODEventParameterController.cs  # FMOD事件参数控制器
│   │   ├── FragmentManager.cs      # 碎片管理器
│   │   ├── FragmentMotionController.cs  # 碎片运动控制器
│   │   ├── GameManager.cs           # 游戏管理器
│   │   ├── LevelManager.cs          # 关卡管理器
│   │   ├── PerformanceMonitor.cs    # 性能监控器
│   │   ├── PlayerController.cs      # 玩家控制器
│   │   ├── PostProcessController.cs # 后处理控制器
│   │   ├── ResonancePoint.cs        # 共振点
│   │   ├── UserInterfaceManager.cs  # UI管理器
│   │   └── VisualFeedbackSystem.cs  # 视觉反馈系统
│   ├── Shaders/                     # 着色器
│   │   └── EmotionFragmentShader.shader  # 情绪碎片着色器
│   ├── Renderer.asset               # 渲染器配置
│   └── URPAsset.asset               # 通用渲染管线资源
├── Packages/                        # 包管理配置（必须保留）
│   └── manifest.json                # 包清单
├── ProjectSettings/                 # 项目设置（必须保留）
│   └── Graphics.unity               # 图形配置
└── README.md                        # 项目说明文档
```

## 目录说明

### 必须保留的目录

- **Assets/** - 包含所有游戏资源，是项目的核心内容
  - **Materials/** - 存放材质资源
  - **Prefabs/** - 存放预制体，包括Fragment.prefab
  - **Scenes/** - 存放游戏场景，包括MainScene.unity
  - **Scripts/** - 存放所有C#脚本代码
  - **Shaders/** - 存放着色器文件
- **Packages/** - 包含项目依赖的包管理配置
  - **manifest.json** - 包清单文件
- **ProjectSettings/** - 包含项目的全局设置
  - **Graphics.unity** - 图形配置

### 不需要包含的目录

- **Library/** - Unity编辑器自动生成的缓存文件，重新打开项目时会自动重新生成
- **UserSettings/** - 用户特定的编辑器设置，删除后Unity会使用默认设置

## 快速开始

### 环境要求

- Unity 编辑器（具体版本请查看 ProjectSettings/ProjectVersion.txt）
- 适当的开发环境（Visual Studio / Visual Studio Code / Rider）
- FMOD Studio（用于音频系统，如果需要编辑音频）

### 打开项目

1. 确保已安装 Unity Hub 和 Unity 编辑器
2. 使用 Unity Hub 打开项目根目录
3. 等待 Unity 完成资源导入

### 运行游戏

1. 在 Unity 编辑器中打开 MainScene.unity 场景
2. 点击播放按钮开始游戏
3. 使用键盘和鼠标进行游戏操作

## 游戏系统

### 情绪系统

游戏核心的情绪状态机（EmotionStateMachine）管理四个情绪维度：

- 压抑（Constraint）
- 焦虑（Anxiety）
- 能动性（Agency）
- 希望（Hope）

### 碎片系统

碎片管理器（FragmentManager）负责：

- 生成和管理场景中的碎片
- 根据情绪状态调整碎片行为
- 实现碎片的运动控制

### 视觉系统

- **EmotionFragmentShader** - 情绪碎片着色器，根据情绪状态改变视觉效果
- **PostProcessController** - 后处理控制器，实现视觉特效
- **VisualFeedbackSystem** - 视觉反馈系统，提供交互反馈

### 音频系统

- **AudioManager** - 音频管理器，控制游戏音效和音乐
- **FMODEventParameterController** - FMOD事件参数控制器，实现动态音频

### UI系统

- **UserInterfaceManager** - UI管理器，控制游戏界面
- **EmotionUIPanel** - 情绪UI面板，显示当前情绪状态

## 开发说明

### 代码规范

- 所有脚本使用中文注释
- 遵循Unity命名规范
- 使用单例模式管理全局系统

### 性能优化

- 使用对象池（ObjectPool）管理碎片对象
- 性能监控器（PerformanceMonitor）实时监控游戏性能
- 根据情绪状态动态调整碎片数量

## 注意事项

- 不要将 Library 目录添加到版本控制系统（如 Git）
- 删除 Library 目录后，首次打开项目需要重新导入资源，可能需要较长时间
- 删除 UserSettings 目录后，Unity 编辑器的个人设置会恢复默认
- 修改情绪系统参数时，注意保持各维度之间的平衡
- 碎片数量会影响游戏性能，建议根据设备性能调整 maxFragmentCount

## 许可证

[请在此处添加许可证信息]
