---
name: desktop-ui-design
description: Guia de boas práticas e padrões para modernização de interfaces desktop (Windows Forms, C#, Siticone/MetroFramework), com foco em Dark Mode premium, hierarquia visual, tipografia moderna e design de painéis/dashboards.
---

# Guia de Design e Modernização de UI Desktop (.NET / Windows Forms)

Este guia estabelece os padrões visuais e arquiteturais para elevar interfaces de aplicações desktop Windows a um padrão moderno, profissional e esteticamente atraente.

## 1. Princípios de Estética & Paleta de Cores (Dark Mode Premium)

Evite tons de preto puro `#000000` absoluto sem contraste e cores primárias saturadas em excesso. Adote camadas de profundidade:

* **Superfície Base (Background)**: `#0E1117` ou `#12151E` (Dark Slate / Midnight Navy)
* **Cartões e Painéis (Elevated Cards)**: `#1A1D27` ou `#1E2230`
* **Bordas Sutis (Borders)**: `#2B3042` (com `BorderThickness = 1`)
* **Cor de Acento Primária**: Ciano Neon (`#00D2FF`) ou Roxo Elétrico (`#7928CA` / `#8A2BE2`)
* **Cores de Estado**:
  - Sucesso / Protegido: Verde Esmeralda (`#00E676` ou `#10B981`)
  - Alerta / Modificado: Âmbar / Laranja (`#F59E0B`)
  - Erro / Perigo: Vermelho Rubi (`#EF4444`)
* **Tipografia**:
  - Texto Primário: Branco Puro (`#FFFFFF`) ou Gelo (`#F3F4F6`)
  - Texto Secundário: Cinza Médio (`#9CA3AF`)
  - Família de Fonte: `Segoe UI`, `Segoe UI Variable` ou `Inter`

## 2. Eliminação de Artefatos Legados

* **Botões de Controle de Janela (Fechar / Minimizar)**:
  - Nunca utilize caracteres brutos como `0`, `1`, `r` (que dependiam de fontes legadas como *Marlett* e quebram no Windows 10/11).
  - Utilize `SiticoneControlBox` ou botões estilizados com ícones Unicode limpos (`✕`, `—`).
* **Layout em Grade Inteligente vs. Botões Flutuantes**:
  - Agrupe funcionalidades por propósito em cartões (`SiticonePanel` com `BorderRadius = 12`).
  - Use abas ou seções claras (ex: **Dashboard**, **Spoofing**, **Segurança**, **Limpeza**).

## 3. Componentes Siticone UI Recomendados

* **Botões**: `SiticoneButton` com `BorderRadius = 8` a `12`, preenchimento gradiente ou sólido escuro com hover brilhante:
  ```csharp
  button.Animated = true;
  button.BorderRadius = 8;
  button.FillColor = Color.FromArgb(26, 29, 39);
  button.HoverState.FillColor = Color.FromArgb(0, 210, 255);
  button.HoverState.ForeColor = Color.Black;
  ```
* **Cartões Informativos**: `SiticonePanel` com `BorderColor = Color.FromArgb(43, 48, 66)` e cantos arredondados.
* **Badges de Status**: Etiquetas visuais como `[PROTEGIDO]` ou `[OFFLINE]` para dar feedback imediato de segurança ao usuário.
