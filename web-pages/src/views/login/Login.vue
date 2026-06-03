<template>
  <div class="login-wrapper">
    <div class="login-card">
      <div class="login-header">
        <img class="logo" src="@/assets/logo_A.png" alt="" />
        <p class="login-subtitle">AI開発プラットフォーム</p>
      </div>

      <div v-if="error" class="error-message">
        <kqi-display-error :error="error" />
      </div>

      <el-form
        ref="loginForm"
        :label-position="labelPosition"
        :rules="rules"
        :model="form"
        class="login-form"
        @submit.native.prevent="handleLogin"
      >
        <el-form-item prop="user" :label-width="labelwidth">
          <el-input
            v-model="form.user"
            placeholder="ユーザ名"
            prefix-icon="el-icon-user"
          />
        </el-form-item>
        <el-form-item prop="password" :label-width="labelwidth">
          <el-input
            v-model="form.password"
            type="password"
            placeholder="パスワード"
            prefix-icon="el-icon-lock"
            show-password
          />
        </el-form-item>
        <el-form-item class="button-group">
          <el-button class="login-button" type="primary" native-type="submit">
            ログイン
          </el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script>
import KqiDisplayError from '@/components/KqiDisplayError'
import { createNamespacedHelpers } from 'vuex'
const { mapActions } = createNamespacedHelpers('account')
const formRule = {
  required: true,
  trigger: 'blur',
  message: '必須項目です',
}

export default {
  title: 'ログイン',
  components: {
    KqiDisplayError,
  },
  data() {
    let err = null
    if (this.$route.query.timeout) {
      err = Error('認証エラー：ログインしてください。')
    }
    return {
      labelwidth: '100px',
      error: err,
      form: {
        user: '',
        password: '',
      },
      rules: {
        user: [formRule],
        password: [formRule],
      },
      returnUrl: this.$route.query.return_url,
      labelPosition: 'top',
    }
  },
  methods: {
    ...mapActions(['login', 'logout']),
    async handleLogin() {
      this.$refs['loginForm'].validate(async valid => {
        if (valid) {
          try {
            await this.login({
              userName: this.form.user,
              password: this.form.password,
            })
            this.error = null
            this.$router.push('/')
          } catch (error) {
            this.handleLogout()
            this.error = error
          }
        }
      })
    },
    async handleLogout() {
      this.logout()
      this.$router.push('/login')
    },
  },
}
</script>

<style lang="scss" scoped>
$primary: #4f46e5;
$primary-light: #6366f1;
$text-primary: #1e293b;
$text-secondary: #64748b;
$border-color: #e2e8f0;

.login-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  position: relative;

  &::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: radial-gradient(
        circle at 20% 50%,
        rgba(99, 102, 241, 0.3) 0%,
        transparent 50%
      ),
      radial-gradient(
        circle at 80% 20%,
        rgba(139, 92, 246, 0.2) 0%,
        transparent 40%
      );
  }
}

.login-card {
  position: relative;
  width: 420px;
  background: #fff;
  border-radius: 16px;
  padding: 48px 40px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}

.login-header {
  text-align: center;
  margin-bottom: 36px;
}

.logo {
  height: 48px;
  margin-bottom: 12px;
}

.login-subtitle {
  color: $text-secondary;
  font-size: 14px;
  font-weight: 500;
  margin: 0;
}

.error-message {
  text-align: center;
  margin-bottom: 20px;
}

.login-form {
  .el-form-item {
    margin-bottom: 22px !important;
  }
}

.button-group {
  text-align: center;
  padding-top: 8px;
}

.login-button {
  width: 100%;
  height: 44px !important;
  font-size: 15px !important;
  font-weight: 600 !important;
  border-radius: 8px !important;
  background: linear-gradient(
    135deg,
    $primary 0%,
    $primary-light 100%
  ) !important;
  border: none !important;
  letter-spacing: 0.05em;
  transition: all 0.3s ease !important;

  &:hover {
    transform: translateY(-1px);
    box-shadow: 0 8px 20px -4px rgba(79, 70, 229, 0.4) !important;
  }

  &:active {
    transform: translateY(0);
  }
}
</style>
