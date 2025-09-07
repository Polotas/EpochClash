# 🏛️ EpochClash

**EpochClash** é um jogo de estratégia em tempo real desenvolvido em Unity onde os jogadores comandam exércitos através de diferentes épocas históricas, desde a Idade da Pedra até a Era Moderna.

## 📖 Sobre o Jogo

EpochClash é um jogo de torre de defesa com elementos de estratégia onde você deve defender sua base enquanto ataca a base inimiga. O jogo apresenta um sistema único de progressão através de diferentes eras históricas, cada uma com suas próprias unidades e características.

### 🎮 Características Principais

- **Sistema de Eras**: Progrida através de 5 eras históricas diferentes
- **Combate Estratégico**: Sistema de combate corpo a corpo e à distância
- **Economia de Recursos**: Gerencie ouro e carne para recrutar unidades
- **Sistema de Upgrades**: Melhore sua base e unidades
- **Audio Imersivo**: Efeitos sonoros e música ambiente
- **Salvamento Automático**: Progresso salvo automaticamente

## 🏛️ Eras do Jogo

1. **Idade da Pedra** (Stone Age) - Homens das cavernas com ferramentas primitivas
2. **Era Espartana** (Spartan Age) - Guerreiros espartanos com lanças e escudos
3. **Era Egípcia** (Egyptian Age) - Soldados egípcios com equipamentos do deserto
4. **Era Medieval** (Medieval Age) - Cavaleiros medievais com armaduras
5. **Era Moderna** (Modern Age) - Soldados modernos com armas avançadas

## ⚔️ Sistema de Combate

### Tipos de Ataque
- **Corpo a Corpo (Melee)**: Unidades que atacam de perto
- **À Distância (Ranged)**: Unidades que atiram projéteis
- **Grandes (Big)**: Unidades especiais com mais poder

### Mecânicas de Jogo
- **IA de Navegação**: Unidades usam NavMesh para movimentação inteligente
- **Sistema de Alvos**: Unidades procuram automaticamente o inimigo mais próximo
- **Combate Automático**: Batalhas acontecem automaticamente quando unidades se encontram
- **Sistema de Vida**: Unidades e bases têm pontos de vida que diminuem com dano

## 💰 Sistema Econômico

- **Ouro**: Moeda principal para comprar unidades e upgrades
- **Carne**: Recurso para acelerar produção de unidades
- **Drops**: Inimigos derrotados dropam moedas
- **Upgrades**: Melhore vida da base e velocidade de produção

## 🛠️ Tecnologias Utilizadas

### Unity Engine
- **Versão**: Unity 2023.x
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Input System**: New Unity Input System
- **NavMesh**: Sistema de navegação de IA

### Plugins e Assets
- **DOTween Pro**: Animações e tweening
- **TextMesh Pro**: Sistema de texto avançado
- **Kenney Audio**: Efeitos sonoros e interface

### Sistemas Implementados
- **Padrão Singleton**: GameManager, EraManager, AudioManager
- **Object Pooling**: Otimização de performance para projéteis e efeitos
- **Sistema de Save/Load**: Persistência de dados em JSON
- **Padrão Observer**: Sistema de eventos para UI e gameplay
- **State Machine**: Gerenciamento de estados das unidades

## 📁 Estrutura do Projeto

```
Assets/
├── Project/                    # Código principal do jogo
│   ├── Art/                   # Assets visuais
│   │   ├── Characters/        # Sprites e animações dos personagens
│   │   ├── UI/               # Elementos da interface
│   │   └── Fonts/            # Fontes do jogo
│   ├── Audio/                # Efeitos sonoros e música
│   ├── Prefab/               # Prefabs organizados por categoria
│   │   ├── Player/           # Unidades do jogador
│   │   ├── Enemies/          # Unidades inimigas
│   │   └── FX/               # Efeitos visuais
│   ├── Scripts/              # Código C#
│   │   ├── Managers/         # Gerenciadores do sistema
│   │   ├── Unit/             # Lógica das unidades
│   │   ├── UI/               # Interface do usuário
│   │   └── Core/             # Sistemas fundamentais
│   ├── Scenes/               # Cenas do Unity
│   └── Settings/             # Configurações do URP
├── Plugin/                   # Plugins de terceiros
└── Resources/                # Recursos carregados dinamicamente
```

## 🎯 Principais Scripts

### Gerenciadores
- `GameManager.cs` - Gerenciador principal do jogo
- `EraManager.cs` - Controle das eras históricas
- `AudioManager.cs` - Sistema de áudio
- `SaveManager.cs` - Persistência de dados

### Gameplay
- `UnitController.cs` - Controle das unidades
- `CombatSystem.cs` - Sistema de combate
- `Base.cs` - Lógica das bases
- `EnemySpawner.cs` / `PlayerSpawner.cs` - Spawn de unidades

### Interface
- `UIGameController.cs` - Interface principal do jogo
- `UIHeathBar.cs` - Barras de vida
- `UISettings.cs` - Menu de configurações

## 🚀 Como Executar

1. **Pré-requisitos**:
   - Unity 2023.x ou superior
   - Visual Studio ou IDE compatível com C#

2. **Instalação**:
   ```bash
   # Clone o repositório
   git clone [url-do-repositorio]
   
   # Abra o projeto no Unity Hub
   # Selecione a pasta do projeto
   ```

3. **Execução**:
   - Abra a cena principal em `Assets/Project/Scenes/SampleScene.unity`
   - Pressione Play no Unity Editor

## 🎮 Controles

- **Mouse**: Navegação pela interface
- **Clique**: Seleção de botões e ações
- **Interface Touch**: Compatível com dispositivos móveis

## 🔧 Configurações

O jogo inclui sistema de configurações para:
- **Volume da Música de Fundo**: Controle do áudio ambiente
- **Volume dos Efeitos**: Controle dos efeitos sonoros
- **Qualidade Gráfica**: Otimizações para diferentes dispositivos

## 💾 Sistema de Save

- **Salvamento Automático**: Progresso salvo automaticamente
- **Dados Persistidos**:
  - Ouro acumulado
  - Era atual
  - Níveis de upgrade
  - Configurações de áudio
  - Unidades desbloqueadas

## 🏗️ Arquitetura do Código

### Padrões Utilizados
- **Singleton**: Para gerenciadores globais
- **Observer**: Para comunicação entre sistemas
- **Object Pool**: Para otimização de performance
- **Component-Based**: Arquitetura modular do Unity

### Principais Sistemas
- **Health System**: Gerenciamento de vida das entidades
- **Combat System**: Lógica de combate e dano
- **Spawning System**: Criação e gerenciamento de unidades
- **UI System**: Interface responsiva e modular

## 🎨 Arte e Design

- **Estilo Visual**: 2D com sprites detalhados
- **Paleta de Cores**: Tons históricos apropriados para cada era
- **Animações**: Sistema de animação fluido com DOTween
- **UI Design**: Interface limpa e intuitiva

## 🔊 Audio

- **Música de Fundo**: Trilha sonora épica e imersiva
- **Efeitos Sonoros**: 
  - Sons de combate realistas
  - Feedback audio para ações da UI
  - Sons ambientes para cada era

## 📱 Plataformas

- **PC**: Windows/Mac/Linux
- **WebGL**: Compatível com navegadores
- **Mobile**: Android/iOS (configuração incluída)

## 🚧 Funcionalidades Futuras

- Sistema multiplayer
- Mais eras históricas
- Campanhas com história
- Sistema de conquistas
- Ranking online

## 👥 Créditos

**Desenvolvido por**: One Gear Studio

### Assets Utilizados
- **DOTween Pro**: Animações
- **Kenney Assets**: Audio e interface
- **Unity Technologies**: Engine e ferramentas

## 📄 Licença

Este projeto é propriedade da One Gear Studio. Todos os direitos reservados.

---

*EpochClash - Conquiste através das eras! 🏛️⚔️*
