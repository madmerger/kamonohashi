<template>
  <div id="app">
    <el-container>
      <el-header>
        <kqi-header @menu="handleMenu" />
      </el-header>
      <el-container class="main-container">
        <kqi-menu v-show="menu" class="sidenav" />
        <el-main>
          <router-view class="content" :class="{ 'content-hidden': !menu }" />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
import KqiHeader from '@/components/KqiHeader'
import KqiMenu from '@/components/KqiMenu'
import Util from '@/util/util'

export default {
  components: {
    KqiHeader,
    KqiMenu,
  },
  data() {
    return {
      menu: this.getMenu(),
    }
  },
  watch: {
    menu() {
      this.setMenu(this.menu)
    },
  },
  created() {
    window.addEventListener('beforeunload', this.setCookieToken)
  },
  destroyed() {
    window.removeEventListener('beforeunload', this.setCookieToken)
  },
  methods: {
    async handleMenu() {
      this.menu = !this.menu
    },
    setMenu(showFlg) {
      Util.setCookie('.Platypus.ShowMenu', showFlg)
    },
    getMenu() {
      let v = Util.getCookie('.Platypus.ShowMenu')
      return v === 'true'
    },
    setCookieToken() {
      let token = this.$store.getters['account/token']
      let cookieTokenKey = '.Platypus.Auth'
      let cookieToken = Util.getCookie(cookieTokenKey, token)

      if (!cookieToken) {
        // Cookieにトークン情報がないときはログアウト
        this.$store.dispatch['account/logout']
      } else {
        // Cookieにトークン情報があるときはCookie情報を更新
        Util.setCookie(cookieTokenKey, token)
      }
    },
  },
}
</script>

<style lang="scss" scoped>
#app {
  color: #1e293b;

  $header-height: 56px;
  $sidebar-width: 240px;
  $sidebar-collapsed: 64px;
  $sidebar-bg: #1e293b;

  .el-header {
    height: $header-height !important;
    position: fixed;
    width: 100%;
    z-index: 10;
    margin-top: 0;
    top: 0;
    left: 0;
    background-color: $sidebar-bg;
    border-bottom: 1px solid rgba(255, 255, 255, 0.08);
    padding: 0;
    box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1),
      0 1px 2px -1px rgba(0, 0, 0, 0.1);
  }

  .sidenav {
    margin-top: $header-height;
    height: calc(100vh - #{$header-height});
    position: fixed;
    z-index: 5;
    top: 0;
    left: 0;
    overflow-x: hidden;
    overflow-y: auto;
    background-color: $sidebar-bg;
    border-right: 1px solid rgba(255, 255, 255, 0.06);
    transition: width 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  }

  @media screen and (max-width: 1000px) {
    .sidenav {
      width: $sidebar-collapsed;
    }
  }
  @media screen and (min-width: 1000px) {
    .sidenav {
      width: $sidebar-width;
    }
  }

  .el-main {
    padding-top: $header-height;
    height: 100vh;
    background-color: #f1f5f9;
    padding-left: 24px;
    padding-right: 24px;
    padding-bottom: 24px;
  }

  .el-transfer-panel {
    width: 40% !important;
  }

  @media screen and (max-width: 1000px) {
    .content {
      margin-left: $sidebar-collapsed;
    }
  }
  @media screen and (min-width: 1000px) {
    .content {
      margin-left: $sidebar-width;
    }
  }
}

.content-hidden {
  margin-left: 0px !important;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter,
.fade-leave-to {
  opacity: 0;
}
</style>

<style src="./reset.css" />
<style src="./style.css" />
