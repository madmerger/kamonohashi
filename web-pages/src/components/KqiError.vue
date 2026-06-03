<template>
  <div class="parent">
    <p class="parent2">
      <el-card class="box-card frame">
        <el-row>
          <el-col :span="11" class="frame-image">
            <span style="font-size: 250px; vertical-align: middle;" />
            <img
              v-if="status === 404"
              class="img"
              src="@/assets/error_404.png"
              width="300px"
              alt=""
            />
            <img
              v-else-if="status === 503"
              class="img"
              src="@/assets/error_503.png"
              width="300px"
              alt=""
            />
            <img
              v-else
              src="@/assets/error.png"
              class="img"
              width="300px"
              alt=""
            />
          </el-col>
          <el-col :span="13" class="frame-text">
            <div class="error-message">
              <h2>{{ $t('messages.error_page_title') }}</h2>
              <h5>
                <span v-if="url">
                  {{ $t('messages.error_page_not_found', { url: url }) }}<br />
                </span>
                <span v-if="status">
                  {{ $t('messages.error_code', { status: status }) }}<br />
                </span>
                <span v-if="message">
                  {{ $t('messages.error_message', { message: message }) }}<br />
                </span>
              </h5>
              <h4>
                {{ $t('messages.error_resolution') }}<br />
                <br />
                {{ $t('messages.error_retry') }}<br />
                <br />
                {{ $t('messages.error_manual') }}<br />
                <br />
                {{ $t('messages.error_server') }}<br />
                {{ $t('messages.error_patience') }}
              </h4>
            </div>
          </el-col>
        </el-row>
      </el-card>
    </p>
  </div>
</template>

<script>
export default {
  title() {
    return this.$t('titles.error')
  },

  data() {
    return {
      url: this.$route.query.url,
      status: this.$route.query.status,
      message: this.$route.query.message,
    }
  },
  watch: {
    $route() {
      this.url = this.$route.query.url
      this.status = this.$route.query.status
      this.message = this.$route.query.message
    },
  },
}
</script>

<style lang="scss" scoped>
.parent {
  position: relative;
  height: 600px;
}

.parent2 {
  position: absolute;
  top: 50%;
  left: 50%;
  -ms-transform: translate(-50%, -50%);
  -webkit-transform: translate(-50%, -50%);
  transform: translate(-50%, -50%);
  width: 800px;
  text-align: center;
}

.frame {
  border: 1px solid gray;
  border-color: rgb(235, 238, 245) rgb(235, 238, 245) rgb(235, 238, 245)
    rgb(26, 191, 213);
  border-style: solid;
  border-width: 1px 1px 1px 20px;
  border-image: none 100% / 1 / 0 stretch;
}

.frame-image {
  vertical-align: middle;
}

.frame-text {
  border-left: dashed rgb(26, 191, 213) 2px;
  padding-left: 20px;
}

.img {
  vertical-align: middle;
}

.error-message {
  text-align: left;
}
</style>
