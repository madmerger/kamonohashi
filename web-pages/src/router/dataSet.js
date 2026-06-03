import Index from '@/views/dataSet/Index'
import Edit from '@/views/dataSet/Edit'
import Preview from '@/views/dataSet/Preview'

export default [
  {
    path: '/dataset',
    component: Index,
    children: [
      {
        path: 'create/:id?',
        component: Edit,
        props: true,
      },
      {
        path: 'edit/:id',
        component: Edit,
        props: true,
      },
      {
        path: 'preview/:id',
        component: Preview,
        props: true,
      },
    ],
  },
]
