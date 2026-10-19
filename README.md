# SecHex-Spoofy 🔑︎

[![Downloads](https://img.shields.io/github/downloads/SecHex/SecHex-Spoofy/total)](https://github.com/SecHex/SecHex-Spoofy)
[![Plataforma](https://img.shields.io/badge/Plataforma-Windows%2010%20%7C%2011-blue.svg)](https://microsoft.com)
[![Framework](https://img.shields.io/badge/.NET-6.0--windows-purple.svg)](https://dotnet.microsoft.com/)
[![Finalidade](https://img.shields.io/badge/Finalidade-Educacional-brightgreen.svg)](#aviso-educacional-e-finalidade-de-estudo-)
[![Segurança](https://img.shields.io/badge/Acesso%20Remoto-Bloqueado-red.svg)](#-segurança-e-bloqueio-de-acesso-remoto)

Ferramenta didática para estudo de identificadores de hardware (HWID), telemetria do sistema operacional Windows e mecanismos de segurança defensiva.

---

### 🎓 Aviso Educacional e Finalidade de Estudo

> **IMPORTANTE:** Este projeto foi desenvolvido **estritamente para fins educacionais, de pesquisa acadêmica e aprendizado** sobre a arquitetura interna do Microsoft Windows, estruturas do Registro, chamadas WMI, identificadores de rede e funcionamento de telemetrias.

---

## 💻 Compatibilidade

* **Windows 11** (Versão 22H2 / Builds 22621+) ✅
* **Windows 10** (Versão 22H2 / Builds 19045+) ✅

---

## 🛡️ Segurança e Bloqueio de Acesso Remoto

Para garantir a privacidade e segurança total da máquina durante a execução:

* **100% Offline e Local**: O aplicativo não realiza chamadas externas, não abre portas na rede e não possui telemetria oculta.
* **Desativação de Área de Trabalho Remota (RDP)**: Configura `fDenyTSConnections = 1` no Registro para impedir conexões RDP.
* **Desativação de Assistência Remota**: Define `fAllowToGetHelp = 0` no Registro.
* **Desativação de Serviços Remotos**: Para e desativa os serviços `RemoteRegistry` (Registro Remoto), `WinRM` (Gerenciamento Remoto via PowerShell) e `TermService`.
* **Regras de Bloqueio no Firewall do Windows**: Cria regras ativas bloqueando o tráfego de entrada nas portas **3389** (RDP) e **5985/5986** (WinRM).

---

## 🏛️ Arquitetura do Projeto

O projeto foi refatorado e organizado em uma arquitetura modular moderna:

```
SecHex-Spoofy/
├── SecHex.sln                   # Solução unificada para Visual Studio / VS Code
├── SecHex.Core/                 # Biblioteca central compartilhada (.NET 6 Windows)
│   ├── Helpers/                 # ProcessHelper, RandomHelper (MAC LAA válido)
│   ├── Models/                  # SystemInformation, OperationResult
│   └── Services/                # Serviços de Spoofing, Limpeza, Backup e Segurança
├── SecHex-CMDLINE/RPZ-HWID/     # Versão de linha de comando (CLI interativo)
└── SecHex-GUI/SecHex-GUI/       # Interface gráfica desktop em Windows Forms
```

---

## ⚡ Funcionalidades

### 1. Modificação de Identificadores (Spoofing)
* **Discos e Controladoras SCSI**: Atualiza identificadores seriais de periféricos de disco no Registro (`HARDWARE\DEVICEMAP\Scsi` e `DiskPeripheral`).
* **GUIDs de Sistema e Perfil**: Gera novos GUIDs para `HwProfileGuid`, `MachineGuid` e `MachineId`.
* **Nome do Computador (Hostname)**: Gera um novo nome randômico e atualiza as chaves `ComputerName`, `ActiveComputerName` e parâmetros do `Tcpip`.
* **Endereço MAC**: Gera endereços MAC válidos com o bit local (LAA) habilitado e reinicia o adaptador de rede correspondente para aplicar a mudança.
* **Installation ID & Windows Product ID**: Atualiza os IDs de instalação do Windows em `SOFTWARE\Microsoft\Windows NT\CurrentVersion`.
* **Variável EFI**: Gera novo identificador GUID para variáveis EFI em `SYSTEM\CurrentControlSet\Control\Nsi`.
* **Serial SMBIOS**: Altera o valor de `SystemSerialNumber` em `HARDWARE\DESCRIPTION\System\BIOS`.
* **Adaptadores de Vídeo (GPU)**: Identifica dinamicamente os adaptadores da classe Display e atualiza descrições de hardware.

### 2. Limpeza de Sistema e Caches
* **Limpeza Real de Logs de Eventos**: Executa `wevtutil cl` limpando de fato os canais *Application*, *Security*, *System* e *Setup*.
* **Flush de Cache DNS**: Executa `ipconfig /flushdns`.
* **Arquivos Temporários**: Limpeza de resíduos em `%TEMP%` e `C:\Windows\Temp`.
* **Caches de Jogos**: Limpeza de arquivos de cache do inicializador da Ubisoft e do Valorant.
* **Redefinição da Pilha de Rede**: Reseta Winsock, TCP/IP e renova IP, com reaplicação imediata dos bloqueios contra acessos remotos.

### 3. Diagnóstico e Backup
* **Auditoria de Hardware Nativa**: Leitura e exibição segura de CPU, Memória Total (GB), adaptadores de vídeo e endereços MAC sem caminhos inválidos de Linux.
* **Sistema de Backup Atômico**: Salva cópias das chaves alteradas em pastas organizadas por carimbo de data/hora (`yyyyMMdd_HHmmss`) em arquivos `.reg` independentes, permitindo fácil restauração.

---

## 🚀 Como Compilar e Executar

### Pré-requisitos
* **SDK do .NET 6.0** (ou superior) para Windows.
* Privilégios de **Administrador** (necessário para ler e gravar em seções protegidas do Registro e configurar o Firewall).

### Compilação via Terminal
```powershell
# Restaurar dependências e compilar a solução completa
dotnet build SecHex.sln -c Release

# Executar a versão CLI
dotnet run --project SecHex-CMDLINE/RPZ-HWID/RPZ-HWID.csproj

# Executar a versão GUI
dotnet run --project SecHex-GUI/SecHex-GUI/SecHex-GUI.csproj
```

---

## 📸 Prévia da Interface

![Prévia da Interface Gráfica](https://github.com/SecHex/SecHex-Spoofy/assets/96635023/22ad94a2-f003-49cb-b0ef-fea0160bd093)

---

## ⚠️ Isenção de Responsabilidade Legal (Disclaimer)

Leia atentamente este termo antes de usar o código:

1. **Uso por sua Conta e Risco**: Este software é fornecido exclusivamente para fins educacionais, de pesquisa e demonstração prática de conceitos de segurança e sistemas operacionais.
2. **Termos de Terceiros**: A alteração de identificadores de hardware pode violar os termos de serviço (ToS) de softwares, jogos ou serviços online específicos.
3. **Sem Garantias**: O software é disponibilizado "como está" (*as-is*), sem garantias expressas ou implícitas de qualquer natureza. O autor e colaboradores não se responsabilizam por eventuais danos, perdas de dados ou problemas operacionais decorrentes de seu uso.
4. **Conformidade Legal**: É responsabilidade exclusiva do usuário garantir que o uso desta ferramenta esteja em conformidade com as leis vigentes de sua jurisdição.
