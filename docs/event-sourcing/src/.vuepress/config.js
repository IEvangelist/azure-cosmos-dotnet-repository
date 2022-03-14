const { description } = require('../../package')

module.exports = {
  base: '/cosmos-event-sourcing-docs/',
  title: 'Cosmos Event Sourcing',
  description: description,
  head: [
    ['meta', { name: 'theme-color', content: '#3eaf7c' }],
    ['meta', { name: 'apple-mobile-web-app-capable', content: 'yes' }],
    ['meta', { name: 'apple-mobile-web-app-status-bar-style', content: 'black' }]
  ],
  themeConfig: {
    logo: '/small-logo.jpg',
    repo: 'https://github.com/IEvangelist/azure-cosmos-dotnet-repository',
    editLinks: true,
    docsDir: '',
    editLinkText: 'Contribute to this page',
    lastUpdated: true,
    nav: [
      {
        text: 'Docs',
        link: '/getting-started/',
      },
      {
        text: 'Samples',
        link: '/samples/'
      },
      {
        text: 'Discord',
        link: 'https://discord.com/invite/qMXrX4shAv'
      }
    ],
    sidebar: [
      {
        title: 'Getting Started',
        path: '/getting-started/',
        collapsable: false,
        children: [
          '/getting-started/',
          {
            title: 'Guide',
            path: '/getting-started/guide/00-overview/',
            collapsable: false,
            children: [
              '/getting-started/guide/01-domain-implementation',
              '/getting-started/guide/02-store-config',
              '/getting-started/guide/03-replaying-events',
              '/getting-started/guide/04-read-projection'
            ]
          },
        ]
      },
      {
        title: 'Event Store',
        path: '/event-store/',
        collapsable: false,
        children: [
          '/event-store/persistence/',
          '/event-store/reading/',
        ]
      },
      {
        title: 'Projections',
        path: '/projections/',
        collapsable: false
      },
      {
        title: 'Learning Resources',
        path: '/learning/',
        collapsable: false
      }
    ]
  },

  /**
   * Apply plugins，ref：https://v1.vuepress.vuejs.org/zh/plugin/
   */
  plugins: [
    '@vuepress/plugin-back-to-top',
    '@vuepress/plugin-medium-zoom',
  ]
}
