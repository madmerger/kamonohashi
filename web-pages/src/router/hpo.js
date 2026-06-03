import Index from '@/views/hpo/Index'
import Create from '@/views/hpo/Create'
import Detail from '@/views/hpo/Detail'

export default [
  {
    path: '/hpo',
    component: Index,
    children: [
      {
        path: 'run',
        component: Create,
      },
      {
        path: ':id',
        component: Detail,
        props: true,
      },
    ],
  },
]
