title
<template>
  <div class="header">
    <el-row justify="center">
      <el-col :span="12" class="title">
        <el-button type="text" class="menu label-color" @click="handleMenu">
          <i class="el-icon-s-fold menu-icon" />
        </el-button>
        <router-link to="/">
          <img class="logo" src="@/assets/KAMONOHASHI_logo_white.png" alt="" />
        </router-link>
      </el-col>
      <el-col :span="12" class="user">
        <el-dropdown
          v-if="isLogined"
          trigger="click"
          @command="handleSwitchTenant"
        >
          <span class="el-dropdown-link user-label">
            <span class="user-avatar">
              <icon name="user" scale="1" class="avatar-icon" />
            </span>
            <span class="user-name">
              {{ omitIfLong(account.userName)
              }}<span v-if="account.userDisplayName">{{
                omitIfLong(account.userDisplayName)
              }}</span>
            </span>
            <span class="tenant-badge">
              {{ omitIfLong(account.selectedTenant.displayName) }}
            </span>
            <i class="el-icon-arrow-down" style="font-size: 12px;" />
          </span>
          <el-dropdown-menu
            slot="dropdown"
            :class="{ scroll: account.tenants.length > 10 }"
          >
            <el-dropdown-item
              v-for="(tenant, index) in account.tenants"
              :key="index"
              :command="tenant.id"
              :class="{
                activeTenant: account.selectedTenant.id === tenant.id,
              }"
            >
              {{ tenant.displayName }}
            </el-dropdown-item>
            <hr />
            <el-dropdown-item key="@setting" command="@setting">
              <i class="el-icon-setting" style="margin-right: 6px;" />
              ユーザ情報設定
            </el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
        <el-button type="text" class="logout-btn" @click="handleLogout">
          <i class="el-icon-switch-button" style="margin-right: 4px;" />
          ログアウト
        </el-button>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('account')

export default {
  name: 'Header',
  computed: {
    ...mapGetters(['account', 'isLogined']),
  },
  methods: {
    ...mapActions(['switchTenant', 'logout']),
    omitIfLong(str) {
      return str.length <= 25 ? str : str.substr(0, 25) + '...'
    },
    async handleSwitchTenant(tenant) {
      if (tenant === '@setting') {
        this.$router.push('/setting')
      } else {
        await this.switchTenant({ tenantId: tenant })
        this.$router.push('/')
      }
    },
    async handleMenu() {
      this.$emit('menu')
    },
    async handleLogout() {
      await this.logout()
      this.$router.push('/login')
    },
  },
}
</script>

<style lang="scss" scoped>
$header-bg: #1e293b;
$header-text: #f8fafc;
$primary: #6366f1;

.header {
  height: inherit;
  display: flex;
  align-items: center;
  padding: 0 16px;
}

.el-row {
  height: inherit;
  width: 100%;
  display: flex;
  align-items: center;
  .el-col {
    height: inherit;
    display: flex;
    align-items: center;
  }
}

.title {
  vertical-align: middle;
  text-align: left;
}

.user {
  justify-content: flex-end;
  text-align: right;
  gap: 8px;
}

.menu {
  margin: 0;
  padding: 8px;
  font-size: 20px;
  color: rgba(248, 250, 252, 0.7) !important;
  border: none !important;
  border-radius: 6px;
  transition: all 0.2s ease;

  &:hover,
  &:focus {
    color: $header-text !important;
    background-color: rgba(255, 255, 255, 0.08) !important;
  }
}

.menu-icon {
  font-size: 20px;
}

.logo {
  height: 28px;
  margin-left: 8px;
  opacity: 0.95;
  transition: opacity 0.2s ease;

  &:hover {
    opacity: 1;
  }
}

.user-avatar {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background-color: rgba(99, 102, 241, 0.2);
  margin-right: 8px;
  flex-shrink: 0;

  .avatar-icon {
    color: #a5b4fc !important;
    width: 16px;
    height: 16px;
  }
}

.user-name {
  color: $header-text;
  font-weight: 500;
  font-size: 13px;
  margin-right: 8px;
}

.tenant-badge {
  display: inline-block;
  background-color: rgba(99, 102, 241, 0.15);
  color: #a5b4fc;
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  margin-right: 4px;
}

.activeTenant {
  font-weight: bold;
  color: $primary;
}

.scroll {
  height: 425px;
  overflow-y: scroll;
}

.user-label {
  color: $header-text !important;
  cursor: pointer;
  display: flex;
  align-items: center;
  padding: 6px 12px;
  border-radius: 8px;
  transition: all 0.2s ease;
  font-size: 13px;

  &:hover,
  &:focus {
    color: $header-text !important;
    background-color: rgba(255, 255, 255, 0.06);
  }
}

.logout-btn {
  color: rgba(248, 250, 252, 0.6) !important;
  font-size: 13px !important;
  font-weight: 500;
  padding: 6px 12px !important;
  border-radius: 6px !important;
  transition: all 0.2s ease;

  &:hover {
    color: $header-text !important;
    background-color: rgba(255, 255, 255, 0.08) !important;
  }
}
</style>
