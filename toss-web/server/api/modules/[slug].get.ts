import modules from '~/server/data/modules'

export default defineEventHandler((event) => {
  const slug = getRouterParam(event, 'slug')

  if (!slug) {
    throw createError({ statusCode: 400, statusMessage: 'Module slug is required.' })
  }

  const data = modules[slug]

  if (!data) {
    throw createError({
      statusCode: 404,
      statusMessage: `Module "${slug}" was not found.`
    })
  }

  return data
})






