# Restore - Aplicação E-commerce

Uma aplicação completa de e-commerce, com backend em .NET Core e frontend em React + TypeScript.

---

## Diagrama C4 - Contexto do Sistema

O diagrama abaixo mostra a visão geral dos principais componentes e integrações do sistema Restore:

```mermaid
C4Context
    title Diagrama de Contexto do Sistema - Restore E-commerce
    Enterprise_Boundary(b0, "Restore E-commerce") {
      Person(user, "Usuário", "Cliente que navega, compra e paga pelo site.")
      System_Boundary(s1, "Sistema Restore") {
        System(api, "API .NET Core", "Backend RESTful que gerencia produtos, usuários, pedidos e pagamentos.")
        System(client, "Frontend React", "Interface web para navegação, busca, compra e checkout.")
        SystemDb(db, "Banco de Dados PostgreSQL", "Armazena produtos, usuários, pedidos, etc.")
        SystemDb(redis, "Redis Cache", "Cache em memória para melhorar performance.")
        SystemDb(rabbitmq, "RabbitMQ", "Sistema de mensageria para processamento assíncrono.")
        SystemDb(kafka, "Apache Kafka", "Sistema de streaming de dados para analytics e eventos em tempo real.")
        System_Ext(stripe, "Stripe API", "Serviço externo de pagamentos.")
      }
    }
    Rel(user, client, "Usa via navegador")
    Rel(client, api, "Faz requisições HTTP (REST)")
    Rel(api, db, "ORM/SQL")
    Rel(api, redis, "Cache de dados")
    Rel(api, rabbitmq, "Mensagens assíncronas")
    Rel(api, kafka, "Streaming de eventos")
    Rel(api, stripe, "Integração para pagamentos")
    BiRel(api, client, "Retorna dados e status")
```

### Como funciona
- O **usuário** acessa o sistema pelo navegador, utilizando o frontend em React.
- O **frontend** se comunica com a **API .NET Core** via requisições HTTP (REST), enviando e recebendo dados de produtos, usuários, pedidos, etc.
- A **API** utiliza o **PostgreSQL** para armazenar e recuperar informações do sistema.
- O **Redis** é usado como cache em memória para melhorar a performance de consultas frequentes.
- O **RabbitMQ** processa mensagens de forma assíncrona (pedidos, emails, etc.).
- O **Apache Kafka** processa eventos em tempo real para analytics e comportamento do usuário.
- Para pagamentos, a **API** integra com o serviço externo **Stripe**, processando transações de forma segura.

---

## Estrutura do Projeto

- `API/` - Backend .NET Core Web API
- `client/` - Frontend React + TypeScript

## Como rodar o projeto

### Pré-requisitos
- Docker instalado (para PostgreSQL, Redis e RabbitMQ)
- .NET 9.0 SDK
- Node.js 18+

### Backend
1. **Inicie os serviços com Docker:**
```bash
docker-compose up -d
```

2. Acesse a pasta `API`
3. Copie o arquivo `appsettings.Development.template.json` para `appsettings.Development.json`
4. Preencha as configurações com seus dados reais:
   - String de conexão do banco
   - String de conexão do Redis (já configurada no template)
   - Configurações do RabbitMQ (já configuradas no template)
   - Chaves da API Stripe (disponíveis no [Painel Stripe](https://dashboard.stripe.com/apikeys))

```bash
cd API
cp appsettings.Development.template.json appsettings.Development.json
# Edite o appsettings.Development.json com seus dados
```

5. Execute a API:
```bash
dotnet run
```

### Frontend
1. Acesse a pasta `client`
2. Instale as dependências:
```bash
npm install
```
3. Inicie o servidor de desenvolvimento:
```bash
npm run dev
```

## Nota de Segurança
O arquivo `appsettings.Development.json` está no `.gitignore` e **não deve ser enviado ao GitHub**. Use sempre o arquivo template para criar sua configuração local.

---

## Funcionalidades
- Autenticação e autorização de usuários
- Catálogo de produtos com filtros e busca
- Carrinho de compras
- Integração com Stripe para pagamentos
- Gestão de pedidos
- Interface responsiva
- **Cache Redis** para melhorar performance
- **Mensageria RabbitMQ** para processamento assíncrono
- **Streaming Kafka** para analytics em tempo real

## Tecnologias Utilizadas

### Backend
- .NET Core 8
- Entity Framework Core
- PostgreSQL
- Redis (Cache)
- RabbitMQ (Mensageria)
- Apache Kafka (Streaming)
- Stripe API

### Frontend
- React 18
- TypeScript
- Material-UI
- Redux Toolkit
- React Router
- Stripe Elements 

---

## Redis Cache

O projeto utiliza **Redis** como sistema de cache para melhorar a performance da aplicação.

### O que é Redis?
Redis é uma **"memória super rápida"** que armazena dados frequentemente acessados, reduzindo o tempo de resposta e a carga no banco de dados.

### Como funciona no projeto:
- **Cache de produtos**: Listas de produtos ficam em cache por 10 minutos
- **Cache de produto individual**: Produtos específicos ficam em cache por 30 minutos  
- **Cache de filtros**: Marcas e tipos ficam em cache por 1 hora
- **Invalidação automática**: Cache é limpo quando produtos são criados/atualizados/deletados

### Benefícios:
- ⚡ **Performance**: Produtos carregam muito mais rápido
- 📊 **Escalabilidade**: Menos consultas ao banco de dados
- 🎯 **Experiência do usuário**: Menos tempo de espera
- 🔄 **Flexibilidade**: Cache automático com expiração

### Exemplo prático:
```csharp
// Primeira busca: vai no banco (lento)
var products = await _context.Products.ToListAsync(); // 2-3 segundos

// Segunda busca: vai no Redis (rápido)
var cachedProducts = await _cacheService.GetAsync("products"); // 0.001 segundos
```

### Configuração:
O Redis já está configurado no `docker-compose.yml` e será iniciado automaticamente com:
```bash
docker-compose up -d
```

---

## RabbitMQ (Mensageria)

O projeto utiliza **RabbitMQ** como sistema de mensageria para processamento assíncrono de tarefas.

### O que é RabbitMQ?
RabbitMQ é um **"carteiro inteligente"** que entrega mensagens entre diferentes partes do sistema. Permite processar tarefas em background sem afetar a performance da aplicação.

### Como funciona no projeto:
- **Pedidos**: Quando um pedido é criado, uma mensagem é enviada para processamento assíncrono
- **Emails**: Confirmações de pedido são enviadas em background
- **Estoque**: Atualizações de estoque são processadas sem bloquear a aplicação
- **Faturas**: Geração de faturas acontece em background

### Benefícios:
- ⚡ **Performance**: Aplicação não fica bloqueada esperando processamento
- 📊 **Escalabilidade**: Pode processar milhares de mensagens por segundo
- 🔄 **Confiabilidade**: Mensagens não se perdem, mesmo se o sistema cair
- 🎯 **Desacoplamento**: Sistemas funcionam independentemente

### Exemplo prático:
```csharp
// Quando um pedido é criado
var message = new OrderCreatedMessage
{
    OrderId = order.Id,
    CustomerEmail = user.Email,
    TotalAmount = order.Total,
    CreatedAt = DateTime.UtcNow
};

// Envia para processamento assíncrono
await _messageService.PublishMessageAsync(message);
```

### Configuração:
O RabbitMQ já está configurado no `docker-compose.yml` e será iniciado automaticamente com:
```bash
docker-compose up -d
```

### Interface de Monitoramento:
- **URL**: http://localhost:15672
- **Usuário**: guest
- **Senha**: guest

---

## Apache Kafka (Streaming de Dados)

O projeto utiliza **Apache Kafka** como sistema de streaming de dados para processamento de eventos em tempo real.

### O que é Apache Kafka?
Kafka é um **"rio de dados em tempo real"** que processa milhões de eventos por segundo. Diferente do RabbitMQ (que é como um correio), o Kafka é como um stream contínuo de dados onde você pode "pescar" as informações que precisa.

### Como funciona no projeto:
- **Eventos de Usuário**: Rastreia comportamento do usuário (cliques, navegação, buscas)
- **Eventos de Pedido**: Monitora criação e atualização de pedidos
- **Eventos de Busca**: Analisa termos de busca e resultados
- **Analytics em Tempo Real**: Processa dados para dashboards e relatórios

### Benefícios:
- ⚡ **Performance Extrema**: Processa milhões de eventos por segundo
- 📊 **Retenção de Dados**: Mantém eventos por dias/semanas
- 🔄 **Escalabilidade**: Fácil expansão horizontal
- 🎯 **Resistência a Falhas**: Dados replicados automaticamente
- 📈 **Analytics Avançados**: Análise de comportamento em tempo real

### Exemplo prático:
```csharp
// Evento de comportamento do usuário
var userEvent = new UserEvent
{
    UserId = "user-123",
    EventType = "product_view",
    ProductId = "boot-redis1",
    PageUrl = "/products/boot-redis1",
    Timestamp = DateTime.UtcNow
};

// Envia para processamento em tempo real
await _kafkaService.PublishUserEventAsync(userEvent);
```

### Configuração:
O Kafka já está configurado no `docker-compose.yml` e será iniciado automaticamente com:
```bash
docker-compose up -d
```

### Interface de Monitoramento:
- **URL**: http://localhost:8081
- **Acesso**: Direto (sem autenticação)

### Tópicos Kafka:
- **user-events**: Eventos de comportamento do usuário
- **order-events**: Eventos relacionados a pedidos
- **search-events**: Eventos de busca e navegação

### Consumidores:
- **UserEventsConsumer**: Processa eventos de usuário
- **OrderEventsConsumer**: Processa eventos de pedido
- **SearchEventsConsumer**: Processa eventos de busca

### Endpoint de Demonstração:
```bash
POST http://localhost:5000/api/orders/kafka-demo
```
Gera eventos únicos para teste do sistema Kafka.

---

## 🐳 Docker e Serviços

### Estrutura de Containers
Todos os serviços estão agrupados na rede `restore-tools-network`:

- **restore-tools-postgres**: Banco de dados PostgreSQL
- **restore-tools-redis**: Cache Redis
- **restore-tools-rabbitmq**: Sistema de mensageria RabbitMQ
- **kafka**: Sistema de streaming Apache Kafka
- **zookeeper**: Coordenador do Kafka
- **kafka-ui**: Interface de monitoramento do Kafka

### Comandos Docker Úteis

#### Iniciar todos os serviços:
```bash
docker-compose up -d
```

#### Verificar status:
```bash
docker-compose ps
```

#### Ver logs:
```bash
# Todos os serviços
docker-compose logs

# Serviço específico
docker-compose logs restore-tools-postgres
docker-compose logs restore-tools-redis
docker-compose logs restore-tools-rabbitmq
docker-compose logs kafka
docker-compose logs zookeeper
```

#### Parar todos os serviços:
```bash
docker-compose down
```

#### Acessar containers:
```bash
# PostgreSQL
docker exec -it restore-postgres psql -U postgres -d restore

# Redis
docker exec -it restore-redis redis-cli -a {sua_senha}

# RabbitMQ (bash)
docker exec -it restore-rabbitmq bash
```

#### Testar serviços:
```bash
# Testar PostgreSQL
docker exec -it restore-postgres psql -U postgres -d restore -c "SELECT version();"

# Testar Redis
docker exec -it restore-redis redis-cli -a {sua_senha} PING

# Testar RabbitMQ
# Acesse http://localhost:15672 e faça login com guest/guest
```

### Configurações dos Serviços

| Serviço | Porta | Usuário | Senha | Interface |
|---------|-------|---------|-------|-----------|
| PostgreSQL | 5432 | postgres | {sua_senha} | - |
| Redis | 6379 | - | {sua_senha} | - |
| RabbitMQ | 5672/15672 | guest | {sua_senha} | http://localhost:15672 |

### Volumes e Persistência
- **restore_postgres_data**: Dados do PostgreSQL
- **restore_redis_data**: Dados do Redis
- **restore_rabbitmq_data**: Dados do RabbitMQ

### Rede Docker
Todos os containers estão na rede `restore-network`, permitindo comunicação interna entre os serviços.

