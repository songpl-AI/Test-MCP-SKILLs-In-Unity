# 需求分析与开发计划：怪物与战斗系统 (Monster & Combat System)

## 1. 核心目标
在现有主角移动和相机控制的基础上，增加怪物实体及基础战斗系统，实现主角与怪物的相互攻击和生命值扣减。

## 2. 需求拆解与约束

### 2.1 怪物实体 (Monster Entity)
- **视觉表现**：使用项目中现有的 `Mushroom_01` 模型或基础胶囊体作为怪物形象。
- **基本属性**：生命值 (Health)、攻击力 (Attack Damage)、移动速度 (Move Speed)。
- **AI 行为**：
    - **待机/巡逻 (Idle/Patrol)**：在一定范围内随机移动或静止。
    - **追逐 (Chase)**：当主角进入侦测范围时，追向主角。
    - **攻击 (Attack)**：当主角进入攻击范围时，对主角造成伤害。

### 2.2 主角战斗 (Player Combat)
- **攻击输入**：鼠标左键 (Fire1) 或特定按键触发攻击。
- **攻击判定**：简单的距离/范围判定 (Physics.OverlapSphere 或 Distance Check)。
- **受击反馈**：
    - 扣除生命值。
    - **视觉特效**：在受击位置生成一个红色小方块作为掉血反馈，随后自动消失。
    - 控制台日志输出。

### 2.3 核心系统 (Core Systems)
- **生命值系统 (Health System)**：通用的生命值组件，适用于主角和怪物。
    - 包含 `TakeDamage(int amount)` 接口。
    - 死亡处理 (Die)：怪物销毁，主角重置或提示。
- **伤害接口 (IDamageable)**：定义受击行为的标准接口。

## 3. 技术方案

### 3.1 脚本架构
1.  **`IDamageable.cs`**: 接口，定义 `TakeDamage`。
2.  **`Health.cs`**: 实现 `IDamageable`，管理 HP，处理死亡。
3.  **`MonsterController.cs`**:
    - 状态机：Idle, Chase, Attack。
    - 使用 `NavMeshAgent` (如果场景有 NavMesh) 或简单的 `Vector3.MoveTowards` 进行移动。
    - *注：考虑到场景可能未烘焙 NavMesh，优先使用简单的 Transform 移动或 CharacterController。*
4.  **`PlayerCombat.cs`**:
    - 监听输入。
    - 执行攻击逻辑（检测前方扇形/圆形区域内的 `IDamageable`）。

### 3.2 场景与资源
- **Monster Prefab**: 创建一个包含 `Health` 和 `MonsterController` 的预制体。
- **Scene Setup**: 在场景中放置 1-2 个怪物。

## 4. 验收标准 (Acceptance Criteria)

### 4.1 怪物行为
- [ ] 怪物在场景中可见。
- [ ] 当主角靠近时，怪物会朝向主角移动。
- [ ] 当主角远离时，怪物停止或随机移动。
- [ ] 当怪物接触主角时，主角扣血（Log 输出）。

### 4.2 主角攻击
- [ ] 按下攻击键，主角播放攻击动作（如有）或仅触发攻击逻辑。
- [ ] 攻击范围内的怪物扣血（Log 输出）。
- [ ] **攻击命中时，在怪物位置生成红色方块特效。**
- [ ] 怪物生命值归零后消失（Destroy）。

### 4.3 综合体验
- [ ] 战斗流程闭环：发现怪物 -> 攻击 -> 怪物死亡 / 主角死亡。

## 5. 疑问与假设
- **假设**：场景中没有 NavMesh，怪物移动将使用简单的 `Vector3.MoveTowards` 并贴合地面（或忽略高度差）。
- **假设**：目前不需要复杂的动画系统，仅通过 Log 和简单的位移/颜色变化（如受击变红）来反馈。
- **疑问**：主角是否有现成的攻击动画？（暂时假设没有，仅逻辑实现）。

## 6. 执行计划 (Phased Implementation)

1.  **Phase 1: 基础组件 (Foundation)**
    - 创建 `IDamageable` 和 `Health` 脚本。
2.  **Phase 2: 怪物实现 (Monster)**
    - 创建 `MonsterController` 脚本。
    - 在场景中配置怪物对象。
3.  **Phase 3: 战斗逻辑 (Combat Logic)**
    - 创建 `PlayerCombat` 脚本并挂载到主角。
    - 实现攻击判定与伤害传递。
    - **实现掉血特效逻辑（生成红色方块）。**
4.  **Phase 4: 验证与调试 (Verify)**
    - 运行场景，测试追逐和战斗流程。
